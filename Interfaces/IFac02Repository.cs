using BackEnd.Data;

namespace BackEnd.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de Fac_02_Cuotas.
    /// </summary>
    public interface IFac02Repository
    {
        /// <summary>
        /// Obtiene todos los registros de Fac_02_Cuotas.
        /// </summary>
        /// <returns>Una lista de Fac_02_Cuotas.</returns>
        Task<List<Fac_02_Cuotas>> GetAllAsync();

        /// <summary>
        /// Obtiene un registro de Fac_02_Cuotas por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <returns>El registro de Fac_02_Cuotas.</returns>
        Task<Fac_02_Cuotas> GetByIdAsync(string id);

        /// <summary>
        /// Agrega un nuevo registro de Fac_02_Cuotas.
        /// </summary>
        /// <param name="entity">La entidad a agregar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task AddAsync(Fac_02_Cuotas entity, string usuarioSigo);

        /// <summary>
        /// Actualiza un registro existente de Fac_02_Cuotas.
        /// </summary>
        /// <param name="entity">La entidad a actualizar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task UpdateAsync(Fac_02_Cuotas entity, string usuarioSigo);

        /// <summary>
        /// Elimina un registro de Fac_02_Cuotas por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task DeleteAsync(string id, string usuarioSigo);

        /// <summary>
        /// Verifica si un registro de Fac_02_Cuotas existe por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <returns>Un valor que indica si el registro existe.</returns>
        Task<bool> ExistsByIdAsync(string id);

        /// <summary>
        /// Obtiene el tratamiento de una cuota por su ID.
        /// </summary>
        /// <param name="cuotaId">El ID de la cuota.</param>
        /// <returns>El tratamiento de la cuota.</returns>
        Task<string> GetTratamientoByCuotaIdAsync(string cuotaId);

        /// <summary>
        /// Obtiene el tipo de una cuota por su ID.
        /// </summary>
        /// <param name="cuotaId">El ID de la cuota.</param>
        /// <returns>El tipo de la cuota.</returns>
        Task<string> GetTipoCuotaByCuotaIdAsync(string cuotaId);

        /// <summary>
        /// Verifica si una cuota única existe para una especie y año.
        /// </summary>
        /// <param name="cuotaId">El ID de la cuota.</param>
        /// <param name="especieId">El ID de la especie.</param>
        /// <param name="año">El año de la cuota.</param>
        /// <returns>Un valor que indica si la cuota única existe.</returns>
        Task<bool> ExistsCuotaUnicaAsync(string cuotaId, string especieId, int año);

        /// <summary>
        /// Obtiene la secuencia máxima de una cuota.
        /// </summary>
        /// <param name="cuotaId">El ID de la cuota.</param>
        /// <param name="especieId">El ID de la especie.</param>
        /// <param name="zonaId">El ID de la zona.</param>
        /// <param name="año">El año de la cuota.</param>
        /// <returns>La secuencia máxima de la cuota.</returns>
        Task<int> GetMaxSecuenciaAsync(string cuotaId, string especieId, string zonaId, int año);
    }
}