namespace BackEnd.Models
{
    /// <summary>
    /// Representa una solicitud de autenticación.
    /// </summary>
    public class AuthRequest
    {
        /// <summary>
        /// Nombre de usuario para la autenticación.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Contraseña para la autenticación.
        /// </summary>
        public string Password { get; set; }
    }
}