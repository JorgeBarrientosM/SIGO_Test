using BackEnd.Data;
using BackEnd.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services
{
    /// <summary>
    /// Servicio para la validación de usuarios.
    /// </summary>
    public class UserValidationService : IUserValidationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="UserValidationService"/>.
        /// </summary>
        /// <param name="context">El contexto de la base de datos.</param>
        /// <param name="configuration">La configuración de la aplicación.</param>
        public UserValidationService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Valida si un usuario existe y está activo.
        /// </summary>
        /// <param name="userId">El ID del usuario a validar.</param>
        /// <returns>Un valor que indica si el usuario es válido.</returns>
        public async Task<bool> ValidateUserAsync(string userId)
        {
            // Validar que el usuario existe y está activo
            var user = await _context.Dim_99_Usuarios
                .FirstOrDefaultAsync(u => u.Usuario_ID == userId && u.Estado == "A");
            if (user == null)
            {
                return false;
            }

            // Obtener la lista de roles autorizados
            var rolesAutorizados = _configuration.GetSection("UsuariosAutorizados").Get<List<string>>();

            // Validar que el usuario tiene permisos para esta operación
            if (rolesAutorizados == null || !rolesAutorizados.Contains(user.TipoUsuario))
            {
                return false;
            }

            return true;
        }
    }
}