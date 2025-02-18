namespace BackEnd.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de validación de usuarios.
    /// </summary>
    public interface IUserValidationService
    {
        /// <summary>
        /// Valida si un usuario existe y está activo.
        /// </summary>
        /// <param name="userId">El ID del usuario a validar.</param>
        /// <returns>Un valor que indica si el usuario es válido.</returns>
        Task<bool> ValidateUserAsync(string userId);
    }
}