using BackEnd.Data;
using BackEnd.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// Repositorio para la entidad Fac_01_Mareas.
    /// </summary>
    public class Fac01Repository : IFac01Repository
    {
        private readonly ApplicationDbContext _context;
        private readonly IStateChangeService _stateChangeService;
        private readonly ILogService _logService;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Fac01Repository"/>.
        /// </summary>
        /// <param name="context">El contexto de la base de datos.</param>
        /// <param name="stateChangeService">El servicio de cambio de estado.</param>
        /// <param name="logService">El servicio de registro de eventos.</param>
        public Fac01Repository(ApplicationDbContext context, IStateChangeService stateChangeService, ILogService logService)
        {
            _context = context;
            _stateChangeService = stateChangeService;
            _logService = logService;
        }

        /// <summary>
        /// Obtiene todos los registros de Fac_01_Mareas.
        /// </summary>
        /// <returns>Una lista de Fac_01_Mareas.</returns>
        public async Task<List<Fac_01_Mareas>> GetAllAsync()
        {
            return await _context.Fac_01_Mareas.ToListAsync();
        }

        /// <summary>
        /// Obtiene un registro de Fac_01_Mareas por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <returns>El registro de Fac_01_Mareas.</returns>
        public async Task<Fac_01_Mareas> GetByIdAsync(string id)
        {
            return await _context.Fac_01_Mareas.FindAsync(id);
        }

        /// <summary>
        /// Agrega un nuevo registro de Fac_01_Mareas.
        /// </summary>
        /// <param name="entity">La entidad a agregar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        public async Task AddAsync(Fac_01_Mareas entity, string usuarioSigo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Fac_01_Mareas.Add(entity);
                await _context.SaveChangesAsync();

                await _logService.LogEventAsync("Fac_01_Mareas", "Agrega Registro", $"Agrega: Marea_ID = '{entity.Marea_ID}'", usuarioSigo, entity);

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error al agregar la marea.", ex);
            }
        }

        /// <summary>
        /// Actualiza un registro existente de Fac_01_Mareas.
        /// </summary>
        /// <param name="entity">La entidad a actualizar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        public async Task UpdateAsync(Fac_01_Mareas entity, string usuarioSigo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                await _logService.LogEventAsync("Fac_01_Mareas", "Actualiza Registro", $"Actualiza: Marea_ID = '{entity.Marea_ID}'", usuarioSigo, entity);

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error al actualizar la marea.", ex);
            }
        }

        /// <summary>
        /// Elimina un registro de Fac_01_Mareas por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        public async Task DeleteAsync(string id, string usuarioSigo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entity = await _context.Fac_01_Mareas.FindAsync(id);
                if (entity != null)
                {
                    await _stateChangeService.ChangeStateAsync(entity, usuarioSigo);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error al cambiar estado de marea.", ex);
            }
        }

        /// <summary>
        /// Verifica si un registro de Fac_01_Mareas existe por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <returns>Un valor que indica si el registro existe.</returns>
        public async Task<bool> ExistsByIdAsync(string id)
        {
            return await _context.Fac_01_Mareas.AnyAsync(e => e.Marea_ID == id);
        }

        /// <summary>
        /// Verifica si un registro de Fac_01_Mareas existe por su ID, número de marea y año.
        /// </summary>
        /// <param name="ccoId">El ID del Centro de Costos.</param>
        /// <param name="numMarea">El número de la marea.</param>
        /// <param name="year">El año de la marea.</param>
        /// <returns>Un valor que indica si el registro existe.</returns>
        public async Task<bool> ExistsByDetailsAsync(string ccoId, int numMarea, int year)
        {
            return await _context.Fac_01_Mareas
                .AnyAsync(m => m.Cco_ID == ccoId && m.NumMarea == numMarea && m.FechaInicio.Year == year);
        }

        /// <summary>
        /// Verifica si existe una marea "En Curso" para el mismo barco.
        /// </summary>
        /// <param name="ccoId">El ID del Centro de Costos.</param>
        /// <returns>Un valor que indica si existe una marea "En Curso".</returns>
        public async Task<bool> ExistsInProgressAsync(string ccoId)
        {
            return await _context.Fac_01_Mareas
                .AnyAsync(m => m.Cco_ID == ccoId && m.EstadoOperativo == "en Curso");
        }
    }
}