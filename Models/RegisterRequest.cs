namespace BackEnd.Models
{
    /// <summary>
    /// Representa una solicitud de registro de usuario.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// ID del usuario.
        /// </summary>
        public string Usuario_ID { get; set; }

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string NombreUsuario { get; set; }

        /// <summary>
        /// Correo electrónico del usuario.
        /// </summary>
        public string CorreoElectronico { get; set; }

        /// <summary>
        /// Tipo de usuario.
        /// </summary>
        public string TipoUsuario { get; set; }

        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        public string Password { get; set; }
    }
}