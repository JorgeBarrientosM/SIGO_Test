using System.Threading.Tasks;

/// <summary>
/// Interfaz para el servicio de cambio de estado.
/// </summary>
public interface IStateChangeService
{
    /// <summary>
    /// Cambia el estado de una entidad de manera asincrónica.
    /// </summary>
    /// <typeparam name="TEntity">El tipo de la entidad.</typeparam>
    /// <param name="entity">La entidad cuyo estado se va a cambiar.</param>
    /// <param name="userId">El ID del usuario que realiza el cambio.</param>
    /// <returns>Una tarea que representa la operación asincrónica.</returns>
    Task ChangeStateAsync<TEntity>(TEntity entity, string userId) where TEntity : class;
}