using BackEnd.Data;
using BackEnd.Interfaces;
using System;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Services
{
    /// <summary>
    /// Servicio para el cambio de estado de entidades.
    /// </summary>
    public class StateChangeService : IStateChangeService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogService _logService;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="StateChangeService"/>.
        /// </summary>
        /// <param name="context">El contexto de la base de datos.</param>
        /// <param name="logService">El servicio de registro de eventos.</param>
        public StateChangeService(ApplicationDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        /// <summary>
        /// Cambia el estado de una entidad de manera asincrónica.
        /// </summary>
        /// <typeparam name="TEntity">El tipo de la entidad.</typeparam>
        /// <param name="entity">La entidad cuyo estado se va a cambiar.</param>
        /// <param name="userId">El ID del usuario que realiza el cambio.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        /// <exception cref="InvalidOperationException">Se lanza si no se encuentra una propiedad con el atributo [Key].</exception>
        public async Task ChangeStateAsync<TEntity>(TEntity entity, string userId) where TEntity : class
        {
            dynamic dynamicEntity = entity;
            dynamicEntity.Estado = dynamicEntity.Estado == "A" ? "I" : "A";
            _context.Update(dynamicEntity); // Asegúrate de marcar la entidad como modificada
            await _context.SaveChangesAsync(); // Asegúrate de guardar los cambios aquí

            // Obtener la propiedad que tiene el atributo [Key]
            var keyProperty = typeof(TEntity).GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());

            if (keyProperty != null)
            {
                var keyName = keyProperty.Name;
                var keyValue = keyProperty.GetValue(dynamicEntity);

                // Registro en el log
                await _logService.LogEventAsync(typeof(TEntity).Name, "Cambia Estado", $"Cambia Estado: {keyName} = '{keyValue}'  //  Nuevo Estado = '{dynamicEntity.Estado}'", userId, "");
            }
            else
            {
                throw new InvalidOperationException("No se encontró una propiedad con el atributo [Key].");
            }
        }
    }
}