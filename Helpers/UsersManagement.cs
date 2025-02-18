namespace BackEnd.Helpers
{
    /// <summary>
    /// Clase estática para obtener los roles autorizados de los usuarios.
    /// </summary>
    public static class UsersManagement
    {
        /// <summary>
        /// Obtiene la lista de roles autorizados desde la configuración.
        /// </summary>
        /// <param name="configuration">La configuración de la aplicación.</param>
        /// <returns>Una lista de roles autorizados.</returns>
        public static List<string> ObtenerRolesAutorizados(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var roles = configuration.GetSection("UsuariosAutorizados").Get<List<string>>();
            return roles ?? new List<string>();
        }
    }
}