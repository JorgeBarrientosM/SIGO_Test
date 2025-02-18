using System.Threading.Tasks;

namespace BackEnd.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de registro de eventos.
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// Registra un evento de manera asincrónica.
        /// </summary>
        /// <param name="modulo">El módulo donde ocurrió el evento.</param>
        /// <param name="evento">El nombre del evento.</param>
        /// <param name="detalle">Los detalles del evento.</param>
        /// <param name="usuarioId">El ID del usuario que generó el evento.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task LogEventAsync(string modulo, string evento, string detalle, string usuarioId, object entity);
    }
}