using BackEnd.Data;

namespace BackEnd.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de Fac_01_Mareas.
    /// </summary>
    public interface IFac01Repository
    {
        /// <summary>
        /// Obtiene todos los registros de Fac_01_Mareas.
        /// </summary>
        /// <returns>Una lista de Fac_01_Mareas.</returns>
        Task<List<Fac_01_Mareas>> GetAllAsync();

        /// <summary>
        /// Obtiene un registro de Fac_01_Mareas por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <returns>El registro de Fac_01_Mareas.</returns>
        Task<Fac_01_Mareas> GetByIdAsync(string id);

        /// <summary>
        /// Agrega un nuevo registro de Fac_01_Mareas.
        /// </summary>
        /// <param name="entity">La entidad a agregar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task AddAsync(Fac_01_Mareas entity, string usuarioSigo);

        /// <summary>
        /// Actualiza un registro existente de Fac_01_Mareas.
        /// </summary>
        /// <param name="entity">La entidad a actualizar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task UpdateAsync(Fac_01_Mareas entity, string usuarioSigo);

        /// <summary>
        /// Elimina un registro de Fac_01_Mareas por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task DeleteAsync(string id, string usuarioSigo);

        /// <summary>
        /// Verifica si un registro de Fac_01_Mareas existe por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <returns>Un valor que indica si el registro existe.</returns>
        Task<bool> ExistsByIdAsync(string id);

        /// <summary>
        /// Verifica si un registro de Fac_01_Mareas existe por su ID, número de marea y año.
        /// </summary>
        /// <param name="ccoId">El ID del Centro de Costos.</param>
        /// <param name="numMarea">El número de la marea.</param>
        /// <param name="year">El año de la marea.</param>
        /// <returns>Un valor que indica si el registro existe.</returns>
        Task<bool> ExistsByDetailsAsync(string ccoId, int numMarea, int year);

        /// <summary>
        /// Verifica si existe una marea "En Curso" para el mismo barco.
        /// </summary>
        /// <param name="ccoId">El ID del Centro de Costos.</param>
        /// <returns>Un valor que indica si existe una marea "En Curso".</returns>
        Task<bool> ExistsInProgressAsync(string ccoId);
    }
}