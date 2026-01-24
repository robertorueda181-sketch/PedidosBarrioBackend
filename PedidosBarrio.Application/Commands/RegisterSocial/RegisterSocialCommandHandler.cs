using FluentValidation;
using MediatR;
using PedidosBarrio.Application.Commands.CreateInmueble;
using PedidosBarrio.Application.Commands.CreateNegocio;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Application.Utilities;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace PedidosBarrio.Application.Commands.RegisterSocial
{
    /// <summary>
    /// Handler para registro completo (usuario/contraseña o Google)
    /// Con soporte para transacciones
    /// 1. Valida que email no exista
    /// 2. Crea usuario, empresa y negocio/servicio/inmueble en una transacción
    /// 3. Retorna token JWT
    /// </summary>
    public class RegisterSocialCommandHandler : IRequestHandler<RegisterSocialCommand, LoginResponseDto>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmpresaRepository _empresaRepository;
        private readonly ISuscripcionRepository _suscripcionRepository;
        private readonly IIaModeracionLogRepository _iaModeracionLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IApplicationLogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IValidator<RegisterSocialCommand> _validator;
        private readonly ITextModerationService _moderationService;
        private readonly IPiiEncryptionService _encryptionService;

        public RegisterSocialCommandHandler(
            IUsuarioRepository usuarioRepository,
            IEmpresaRepository empresaRepository,
            ISuscripcionRepository suscripcionRepository,
            IIaModeracionLogRepository iaModeracionLogRepository,
            IUnitOfWork unitOfWork,
            IMediator mediator,
            IJwtTokenService jwtTokenService,
            IApplicationLogger logger,
            IConfiguration configuration,
            IEmailService emailService,
            IValidator<RegisterSocialCommand> validator,
            ITextModerationService moderationService,
            IPiiEncryptionService encryptionService)
        {
            _usuarioRepository = usuarioRepository;
            _empresaRepository = empresaRepository;
            _suscripcionRepository = suscripcionRepository;
            _iaModeracionLogRepository = iaModeracionLogRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
            _configuration = configuration;
            _emailService = emailService;
            _validator = validator;
            _moderationService = moderationService;
            _encryptionService = encryptionService;
        }

        public async Task<LoginResponseDto> Handle(RegisterSocialCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ===== VALIDAR ENTRADA CON FLUENTVALIDATION =====
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    await _logger.LogWarningAsync(
                        $"Validación fallida en registro: {errors} - Email: {_encryptionService.Encrypt(request.Email)}",
                        "RegisterSocialCommand");
                    throw new ValidationException(validationResult.Errors);
                }

                await _logger.LogInformationAsync(
                    $"Iniciando registro: {_encryptionService.Encrypt(request.Email)} - Tipo: {request.TipoEmpresa} - {request.Provider ?? "usuario/contraseña"}",
                    "RegisterSocialCommand");

                // ===== EJECUTAR EN TRANSACCIÓN =====
                var response = await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    return await PerformRegistration(request, cancellationToken);
                });

                return response;
            }
            catch (ValidationException)
            {
                throw; // Re-lanzar excepciones de validación sin modificar
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(
                    $"Error en registro: {ex.Message}",
                    ex,
                    "RegisterSocialCommand");
                throw new ApplicationException($"Error al registrarse: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lógica de registro ejecutada dentro de una transacción
        /// </summary>
        private async Task<LoginResponseDto> PerformRegistration(RegisterSocialCommand request, CancellationToken cancellationToken)
        {
            // ===== 1. VALIDAR EMAIL NO EXISTA EN USUARIOS =====
            var usuarioExistente = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (usuarioExistente != null)
            {
                await _logger.LogWarningAsync(
                    $"Email ya registrado en usuarios: {_encryptionService.Encrypt(request.Email)}",
                    "RegisterSocialCommand");
                throw new ApplicationException($"El email {request.Email} ya está registrado.");
            }

            // ===== 3. CREAR USUARIO =====
            string contrasenaHash = "";
            string contrasenaSalt = "";

            // Si es registro por usuario/contraseña (no Google)
            // La validación ya se hizo con FluentValidation
            if (string.IsNullOrEmpty(request.Provider))
            {
                var (hash, salt) = PasswordHasher.HashPassword(request.Contrasena);
                contrasenaHash = hash;
                contrasenaSalt = salt;
            }

            var usuario = new Usuario(
                email: request.Email,
                contrasenaHash: contrasenaHash,
                contrasenaSalt: contrasenaSalt)
            {
                Activa = true,
                FechaRegistro = DateTime.UtcNow,
                Provider = request.Provider,
                SocialId = request.SocialId
            };

            await _usuarioRepository.AddAsync(usuario);

            // ===== 3.1 EVALUAR CONTENIDO CON AI =====
            bool autoAprobado = false;
            string evaluacionAi = "No se pudo realizar la moderación";
            try
            {
                var textToModerate = $"{request.Email} {request.Nombre} {request.Apellido} {request.NombreEmpresa} {request.Descripcion} {request.Direccion} {request.Referencia}";
                var moderationResult = await _moderationService.ModerateTextAsync(textToModerate);
                autoAprobado = moderationResult.IsAppropriate;
                
                evaluacionAi = autoAprobado 
                    ? "Contenido apropiado" 
                    : $"Marcado por: {string.Join(", ", moderationResult.ViolationCategories)}";

                if (autoAprobado)
                {
                    await _logger.LogInformationAsync(
                        $"Contenido validado por AI: El registro de {_encryptionService.Encrypt(request.Email)} ha sido auto-aprobado.",
                        "RegisterSocialCommand");
                }
                else
                {
                    await _logger.LogWarningAsync(
                        $"Contenido marcado por AI para revisión: {evaluacionAi}",
                        "RegisterSocialCommand");
                }
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync("Error al moderar texto con AI", ex, "RegisterSocialCommand");
                evaluacionAi = $"Error en moderación: {ex.Message}";
            }

            // ===== 4. CREAR EMPRESA =====
            var empresa = new Empresa(usuario.ID, request.TipoEmpresa)
            {
                Aprobado = autoAprobado
            };
            await _empresaRepository.AddAsync(empresa);
            usuario.EmpresaID = empresa.ID;

            // ===== 4.1 GUARDAR LOG DE IA =====
            try
            {
                var iaLog = new IaModeracionLog
                {
                    EmpresaID = empresa.ID,
                    EsTexto = true,
                    Apropiado = autoAprobado,
                    Evaluacion = evaluacionAi,
                    FechaRegistro = DateTime.UtcNow
                };
                await _iaModeracionLogRepository.AddAsync(iaLog);
            }
            catch (Exception ex)
            {
                // No fallar el registro si el log de IA falla
                await _logger.LogErrorAsync("Error al guardar log de moderación IA", ex, "RegisterSocialCommand");
            }

            // ===== 4.2 CREAR SUSCRIPCIÓN GRATUITA (NIVEL 1) =====
            var suscripcion = new Suscripcion(empresa.ID, 0, null)
            {
                NivelSuscripcion = 1, // Free
                FechaFin = null // Sin vencimiento
            };
            await _suscripcionRepository.AddAsync(suscripcion);

            // ===== 5. CREAR REGISTRO ESPECÍFICO SEGÚN TIPO DE EMPRESA =====
            switch (request.TipoEmpresa)
            {
                case 1: // NEGOCIO
                    var negocioCommand = new CreateNegocioCommand(
                        empresaID: empresa.ID,
                        tiposID: 1,
                        urlNegocio: request.NombreEmpresa.ToLower().Replace(" ", "-"),
                        descripcion: request.Descripcion);
                    
                    var negocioResult = await _mediator.Send(negocioCommand, cancellationToken);
                    await _logger.LogInformationAsync(
                        $"Negocio creado: {negocioResult.NegocioID} - {request.NombreEmpresa}",
                        "RegisterSocialCommand");
                    break;

                case 2: // SERVICIO
                    var servicioCommand = new CreateNegocioCommand(
                        empresaID: empresa.ID,
                        tiposID: 2,
                        urlNegocio: request.NombreEmpresa.ToLower().Replace(" ", "-"),
                        descripcion: request.Descripcion);
                    
                    var servicioResult = await _mediator.Send(servicioCommand, cancellationToken);
                    await _logger.LogInformationAsync(
                        $"Servicio creado: {servicioResult.NegocioID} - {request.NombreEmpresa}",
                        "RegisterSocialCommand");
                    break;

                case 3: // INMUEBLE
                    var inmuebleCommand = new CreateInmuebleCommand(
                        empresaID: empresa.ID,
                        tiposID: 3,
                        precio: 0,
                        medidas: request.Descripcion,
                        ubicacion: request.Direccion,
                        dormitorios: 0,
                        banos: 0,
                        descripcion: request.Descripcion);
                    
                    var inmuebleResult = await _mediator.Send(inmuebleCommand, cancellationToken);
                    await _logger.LogInformationAsync(
                        $"Inmueble creado: {inmuebleResult.InmuebleID} - {request.NombreEmpresa}",
                        "RegisterSocialCommand");
                    break;
            }

            // ===== 6. GENERAR TOKEN =====
            var tokenExpirationMinutes = _configuration.GetSection("Jwt:TokenExpirationMinutes").Value;
            int minutosExpiracion = !string.IsNullOrEmpty(tokenExpirationMinutes) && int.TryParse(tokenExpirationMinutes, out int minutos)
                ? minutos
                : 30;

            var token = _jwtTokenService.GenerateToken(usuario, minutosExpiracion);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            // ===== 7. MAPEAR TIPO DE EMPRESA =====
            var tipoEmpresaStr = request.TipoEmpresa switch
            {
                1 => "NEGOCIO",
                2 => "SERVICIO",
                3 => "INMUEBLE",
                _ => "DESCONOCIDO"
            };

            // ===== 8. RETORNAR RESPUESTA =====
            var response = new LoginResponseDto
            {
                UsuarioID = usuario.ID,
                Email = usuario.Email,
                NombreCompleto = $"{request.Nombre} {request.Apellido}",
                EmpresaID = empresa.ID,
                NombreEmpresa = request.NombreEmpresa,
                TipoEmpresa = tipoEmpresaStr,
                Categoria = request.Categoria,
                Telefono = request.Telefono,
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiracion = DateTime.UtcNow.AddMinutes(minutosExpiracion),
                EsNuevo = true
            };

            await _logger.LogInformationAsync(
                $"Registro completado exitosamente: {_encryptionService.Encrypt(usuario.Email)} - Tipo: {tipoEmpresaStr}",
                "RegisterSocialCommand");

            // ===== 9. ENVIAR EMAIL DE BIENVENIDA Y EVALUACIÓN =====
            try
            {
                var emailEnviado = await _emailService.SendWelcomeEmailAsync(
                    toEmail: usuario.Email,
                    userName: $"{request.Nombre} {request.Apellido}".Trim(),
                    businessName: request.NombreEmpresa,
                    businessType: tipoEmpresaStr
                );

                if (emailEnviado)
                {
                    await _logger.LogInformationAsync(
                        $"Email de bienvenida enviado exitosamente a: {_encryptionService.Encrypt(usuario.Email)}",
                        "RegisterSocialCommand");
                }
                else
                {
                    await _logger.LogWarningAsync(
                        $"No se pudo enviar email de bienvenida a: {_encryptionService.Encrypt(usuario.Email)}",
                        "RegisterSocialCommand");
                }
            }
            catch (Exception emailEx)
            {
                // No fallar el registro si el email falla
                await _logger.LogErrorAsync(
                    $"Error al enviar email de bienvenida a {_encryptionService.Encrypt(usuario.Email)}: {emailEx.Message}",
                    emailEx,
                    "RegisterSocialCommand");
            }

            return response;
        }
    }
}

