using PedidosBarrio.Domain.Enums;

namespace PedidosBarrio.Infrastructure.Authorization
{
    public static class AuthorizationPolicies
    {
        // Políticas por suscripción
        public const string RequireFreePlan = "RequireFreePlan";
        public const string RequireVecinoPlan = "RequireVecinoPlan"; 
        public const string RequireEmpresaPlan = "RequireEmpresaPlan";
        public const string RequirePruebaPlan = "RequirePruebaPlan";

        // Políticas por rol
        public const string RequireAdmin = "RequireAdmin";
        public const string RequireModerator = "RequireModerator";
        public const string RequireUser = "RequireUser";

        // Políticas combinadas (más útiles en la práctica)
        public const string EmpresaOrAdmin = "EmpresaOrAdmin";
        public const string VecinoOrModerator = "VecinoOrModerator";
        public const string PremiumAccess = "PremiumAccess"; // Empresa, Prueba o Admin

        public static class FeatureAccess
        {
            // Features específicas por funcionalidad
            public const string CreateCategories = "CreateCategories";
            public const string ModerateImages = "ModerateImages";
            public const string ModerateText = "ModerateText";
            public const string SendEmails = "SendEmails";
            public const string ManageUsers = "ManageUsers";
            public const string ViewAnalytics = "ViewAnalytics";
        }

        public static Dictionary<string, (TipoSuscripcion suscripcion, UsuarioRol rol)> GetFeaturePolicies()
        {
            return new Dictionary<string, (TipoSuscripcion, UsuarioRol)>
            {
                // Categorías: Solo Empresa o Admin
                [FeatureAccess.CreateCategories] = (TipoSuscripcion.Empresa, UsuarioRol.Admin),
                
                // Moderación de imágenes: Vecino+ o Moderador+
                [FeatureAccess.ModerateImages] = (TipoSuscripcion.Vecino, UsuarioRol.Moderador),
                
                // Moderación de texto: Free (para testing) o Moderador+
                [FeatureAccess.ModerateText] = (TipoSuscripcion.Free, UsuarioRol.Moderador),
                
                // Envío de emails: Empresa o Admin
                [FeatureAccess.SendEmails] = (TipoSuscripcion.Empresa, UsuarioRol.Admin),
                
                // Gestión de usuarios: Solo Admin
                [FeatureAccess.ManageUsers] = (TipoSuscripcion.Empresa, UsuarioRol.Admin),
                
                // Analytics: Vecino+ o Moderador+
                [FeatureAccess.ViewAnalytics] = (TipoSuscripcion.Vecino, UsuarioRol.Moderador)
            };
        }
    }
}