namespace BackEnd.Constants
{
    /// <summary>
    /// Clase estática que contiene mensajes constantes utilizados en la aplicación.
    /// </summary>
    public static class Messages
    {
        /// <summary>
        /// Mensaje para datos inválidos.
        /// </summary>
        public const string DatosInvalidos = "Datos inválidos, debe ingresar todos los datos requeridos.";

        /// <summary>
        /// Mensaje para usuario no válido.
        /// </summary>
        public const string UsuarioNoValido = "Usuario no existe o no tiene permisos suficientes.";

        /// <summary>
        /// Mensaje para registro duplicado.
        /// </summary>
        public const string RegistroDuplicado = "Registro ya existe. No se puede insertar duplicado.";

        /// <summary>
        /// Mensaje para registro insertado exitosamente.
        /// </summary>
        public const string RegistroInsertado = "Registro insertado exitosamente.";

        /// <summary>
        /// Mensaje para registro no encontrado.
        /// </summary>
        public const string RegistroNoExiste = "No se encontró el registro especificado.";

        /// <summary>
        /// Mensaje para registro actualizado exitosamente.
        /// </summary>
        public const string RegistroActualizado = "Registro actualizado exitosamente.";

        /// <summary>
        /// Mensaje para cambio de estado exitoso.
        /// </summary>
        public const string CambioEstado = "Estado actualizado exitosamente.";

        /// <summary>
        /// Mensaje para usuario duplicado.
        /// </summary>
        public const string UsuarioDuplicado = "Usuario ya existe. No se puede insertar duplicado.";

        /// <summary>
        /// Mensaje para restablecimiento de contraseña exitoso.
        /// </summary>
        public const string ResetPassword = "Contraseña restablecida exitosamente y enviada por Correo Electrónico.";

        /// <summary>
        /// Mensaje para cambio de contraseña exitoso.
        /// </summary>
        public const string CambioPassword = "Contraseña actualizada correctamente. Por seguridad, vuelva a iniciar sesión.";

        /// <summary>
        /// Mensaje para usuario creado exitosamente.
        /// </summary>
        public const string UsuarioCreado = "Usuario creado exitosamente. Contraseña temporal enviada por Correo Electrónico.";

        /// <summary>
        /// Mensaje para usuario actualizado exitosamente.
        /// </summary>
        public const string UsuarioActualizado = "Usuario actualizado exitosamente.";

        /// <summary>
        /// Mensaje para campos obligatorios.
        /// </summary>
        public const string CamposObligados = "Todos los campos son obligatorios, corrija y vuelva a intentar.";

        /// <summary>
        /// Mensaje para contraseñas que no coinciden.
        /// </summary>
        public const string PasswordDistintas = "Las contraseñas no coinciden, corrija y vuelva a intentar.";

        /// <summary>
        /// Mensaje para contraseña actual incorrecta.
        /// </summary>
        public const string PasswordActual = "Contraseña actual es incorrecta, corrija y vuelva a intentar.";
    }
}