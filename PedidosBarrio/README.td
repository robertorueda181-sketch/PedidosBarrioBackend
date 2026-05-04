📌 Descripción del Proyecto

Esta aplicación es una API REST desarrollada con .NET (Core 10), diseñada para servir como backend de una aplicación web moderna.

La API expone endpoints seguros, escalables y mantenibles, siguiendo principios de arquitectura limpia, buenas prácticas de desarrollo y patrones de diseño ampliamente adoptados en la industria.

🧱 Arquitectura

El proyecto sigue una arquitectura basada en Clean Architecture, separando responsabilidades en capas:

Domain → Entidades y reglas de negocio
Application → Casos de uso, lógica de aplicación
Infrastructure → Acceso a datos, servicios externos
API → Controladores y configuración

Esto permite:

Bajo acoplamiento
Alta mantenibilidad
Facilidad para testing
🧩 Patrones de Diseño

Se implementa el patrón Mediator usando MediatR, lo que permite desacoplar completamente la lógica de negocio de los controladores.

Beneficios:
Separación de responsabilidades
Código más limpio y organizado
Fácil escalabilidad

Ejemplo de flujo:

Controller → Command/Query → Handler → Response
🔐 Seguridad

La API implementa múltiples mecanismos de seguridad:

Autenticación basada en JWT (JSON Web Tokens)
Autorización por roles y políticas
Validación de entrada de datos
Protección contra ataques comunes (inyección SQL, etc.)
Uso de HTTPS obligatorio
⚙️ Buenas Prácticas

El proyecto sigue estándares modernos de desarrollo:

Uso de DTOs para transferencia de datos
Validaciones con FluentValidation (opcional)
Manejo centralizado de excepciones (Middleware)
Logging estructurado
Principios SOLID
Inyección de dependencias nativa de .NET
🌐 Endpoints y Minimal APIs

La API está diseñada utilizando Minimal APIs, permitiendo un enrutamiento más limpio y un menor consumo de recursos frente a controladores tradicionales.

Ejemplo de estructura:

GET     /api/productos
POST    /api/productos
PUT     /api/productos/{id}
DELETE  /api/productos/{id}

🛡️ Control de Errores

Se utiliza el estándar de Minimal APIs para el manejo de errores:
- Respuestas estandarizadas usando `Results.Problem()` para errores internos o inesperados (RFC 7807).
- `Results.ValidationProblem()` o `Results.BadRequest()` para errores del cliente o de negocio, unificando cómo los clientes consumen las incidencias.
🗄️ Configuración

Las configuraciones del proyecto se gestionan mediante archivos como:

appsettings.json
appsettings.Development.json

También se recomienda externalizar configuraciones sensibles (tokens, cadenas de conexión, etc.).

🚀 Ejecución del Proyecto

Para ejecutar el proyecto:

dotnet restore
dotnet build
dotnet run
🔄 Integración y Despliegue

El proyecto está preparado para integrarse con pipelines de CI/CD y desplegarse en servidores Linux o servicios en la nube.

🛠️ Tecnologías Utilizadas
.NET 10
Entity Framework Core
MediatR
JWT Authentication
SQL Server / PostgreSQL