namespace PedidosBarrio.Domain.Enums
{
    public enum TipoSuscripcion
    {
        Free = 1,
        Vecino = 2, 
        Empresa = 3,
        Prueba = 4
    }

    public enum UsuarioRol
    {
        Usuario = 1,
        Moderador = 2,
        Admin = 3,
        SuperAdmin = 4
    }

    public static class SuscripcionExtensions
    {
        public static string ToDisplayName(this TipoSuscripcion suscripcion)
        {
            return suscripcion switch
            {
                TipoSuscripcion.Free => "Plan Gratuito",
                TipoSuscripcion.Vecino => "Plan Vecino", 
                TipoSuscripcion.Empresa => "Plan Empresa",
                TipoSuscripcion.Prueba => "Plan de Prueba",
                _ => "Desconocido"
            };
        }

        public static bool CanAccessAll(this TipoSuscripcion suscripcion)
        {
            return suscripcion == TipoSuscripcion.Empresa || 
                   suscripcion == TipoSuscripcion.Prueba;
        }

        public static bool IsAdmin(this UsuarioRol rol)
        {
            return rol == UsuarioRol.Admin || rol == UsuarioRol.SuperAdmin;
        }
    }
}