using BackEnd.Data;

namespace BackEnd.Interfaces
{
    /// <summary>
    /// Interfaz Genérica para Tablas Dim01 a Dim13.
    /// </summary>
    public interface IDimGenericRepository<T> where T : class
    {
        /// <summary>
        /// Obtiene todos los registros de DimXX.
        /// </summary>
        /// <returns>Una lista de DimXX.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Obtiene un registro de DimXX por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <returns>El registro de DimXX.</returns>
        Task<T> GetByIdAsync(string id);

        /// <summary>
        /// Agrega un nuevo registro de DimXX.
        /// </summary>
        /// <param name="entity">La entidad a agregar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        Task AddAsync(T entity, string usuarioSigo);

        /// <summary>
        /// Actualiza un registro existente de DimXX.
        /// </summary>
        /// <param name="entity">La entidad a actualizar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        Task UpdateAsync(T entity, string usuarioSigo);

        /// <summary>
        /// Elimina un registro de DimXX por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        Task DeleteAsync(string id, string usuarioSigo);

        /// <summary>
        /// Verifica si un registro de DimXX existe por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <returns>Un valor que indica si el registro existe.</returns>
        Task<bool> ExistsAsync(string id);
    }
    
    /// <summary>
    /// Interfaz para el repositorio de Dim_99_Usuarios.
    /// </summary>
    public interface IDim99Repository
    {
        /// <summary>
        /// Obtiene todos los registros de Dim_99_Usuarios.
        /// </summary>
        /// <returns>Una lista de Dim_99_Usuarios.</returns>
        Task<IEnumerable<Dim_99_Usuarios>> GetAllAsync();

        /// <summary>
        /// Obtiene un registro de Dim_99_Usuarios por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <returns>El registro de Dim_99_Usuarios.</returns>
        Task<Dim_99_Usuarios> GetByIdAsync(string id);

        /// <summary>
        /// Agrega un nuevo registro de Dim_99_Usuarios.
        /// </summary>
        /// <param name="entity">La entidad a agregar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        Task AddAsync(Dim_99_Usuarios entity, string usuarioSigo);

        /// <summary>
        /// Actualiza un registro existente de Dim_99_Usuarios.
        /// </summary>
        /// <param name="entity">La entidad a actualizar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        Task UpdateAsync(Dim_99_Usuarios entity, string usuarioSigo);

        /// <summary>
        /// Cambia el estado de un registro de Dim_99_Usuarios.
        /// </summary>
        /// <param name="entity">La entidad a actualizar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        Task ChangeStateAsync(Dim_99_Usuarios entity, string usuarioSigo);

        /// <summary>
        /// Restablece la contraseña de un registro de Dim_99_Usuarios.
        /// </summary>
        /// <param name="entity">La entidad a actualizar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        Task ResetPassAsync(Dim_99_Usuarios entity, string usuarioSigo);

        /// <summary>
        /// Cambia la contraseña de un registro de Dim_99_Usuarios.
        /// </summary>
        /// <param name="entity">La entidad a actualizar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        Task ChangePassAsync(Dim_99_Usuarios entity, string usuarioSigo);

        /// <summary>
        /// Verifica si un registro de Dim_99_Usuarios existe por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <returns>Un valor que indica si el registro existe.</returns>
        Task<bool> ExistsAsync(string id);
    }
}