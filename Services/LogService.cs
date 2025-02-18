using BackEnd.Data;
using BackEnd.Interfaces;
using BackEnd.Models;
using System;
using System.Threading.Tasks;
using BackEnd.Helpers;

namespace BackEnd.Services
{
    /// <summary>
    /// Servicio para el registro de eventos en la base de datos.
    /// </summary>
    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="LogService"/>.
        /// </summary>
        /// <param name="context">El contexto de la base de datos.</param>
        public LogService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Registra un evento de manera asincrónica.
        /// </summary>
        /// <param name="modulo">El módulo donde ocurrió el evento.</param>
        /// <param name="evento">El nombre del evento.</param>
        /// <param name="detalle">Los detalles del evento.</param>
        /// <param name="usuarioId">El ID del usuario que generó el evento.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        public async Task LogEventAsync(string? modulo, string? evento, string? detalle, string? usuarioId, object entity)
        {
            if (string.IsNullOrEmpty(modulo) || string.IsNullOrEmpty(evento) || string.IsNullOrEmpty(detalle) || string.IsNullOrEmpty(usuarioId))
            {
                throw new ArgumentException("Los parámetros no pueden ser nulos o vacíos.");
            }

            // Obtener el nombre de la tabla (entidad)
            var tableName = entity.GetType().Name;

            // Buscar la descripción en el diccionario
            if (IdDescriptions.Descriptions.TryGetValue(tableName, out var descriptionFieldName))
            {
                // Usar reflexión para obtener el valor del campo de descripción
                var descriptionProperty = entity.GetType().GetProperty(descriptionFieldName);
                var descriptionValue = descriptionProperty?.GetValue(entity)?.ToString();

                // Agregar la descripción al mensaje del log
                detalle += $"  //  Descripción = '{descriptionValue}'";
            }

            // Crear el registro de log
            var log = new Fac_98_Log
            {
                FechaHora = DateTime.Now,
                Modulo = modulo,
                Evento = evento,
                Detalle = detalle,
                Usuario_ID = usuarioId
            };
            _context.Fac_98_Log.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}