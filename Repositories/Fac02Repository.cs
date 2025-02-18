using BackEnd.Data;
using BackEnd.Interfaces;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// Repositorio para la entidad Fac_02_Cuotas.
    /// </summary>
    public class Fac02Repository : IFac02Repository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogService _logService;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Fac02Repository"/>.
        /// </summary>
        /// <param name="context">El contexto de la base de datos.</param>
        /// <param name="logService">El servicio de registro de eventos.</param>
        public Fac02Repository(ApplicationDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        /// <summary>
        /// Obtiene todos los registros de Fac_02_Cuotas.
        /// </summary>
        /// <returns>Una lista de Fac_02_Cuotas.</returns>
        public async Task<List<Fac_02_Cuotas>> GetAllAsync()
        {
            return await _context.Fac_02_Cuotas.ToListAsync();
        }

        /// <summary>
        /// Obtiene un registro de Fac_02_Cuotas por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <returns>El registro de Fac_02_Cuotas.</returns>
        public async Task<Fac_02_Cuotas> GetByIdAsync(string id)
        {
            return await _context.Fac_02_Cuotas.FindAsync(id);
        }

        /// <summary>
        /// Agrega un nuevo registro de Fac_02_Cuotas.
        /// </summary>
        /// <param name="entity">La entidad a agregar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        public async Task AddAsync(Fac_02_Cuotas entity, string usuarioSigo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Fac_02_Cuotas.Add(entity);
                await _context.SaveChangesAsync();

                await _logService.LogEventAsync("Fac_02_Cuotas", "Agrega Registro", $"Agrega: Control_ID = '{entity.Control_ID}'", usuarioSigo, entity);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Actualiza un registro existente de Fac_02_Cuotas.
        /// </summary>
        /// <param name="entity">La entidad a actualizar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        public async Task UpdateAsync(Fac_02_Cuotas entity, string usuarioSigo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                await _logService.LogEventAsync("Fac_02_Cuotas", "Actualiza Registro", $"Actualiza: Control_ID = '{entity.Control_ID}'", usuarioSigo, entity);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Elimina un registro de Fac_02_Cuotas por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        public async Task DeleteAsync(string id, string usuarioSigo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entity = await _context.Fac_02_Cuotas.FindAsync(id);
                if (entity != null)
                {
                    _context.Fac_02_Cuotas.Remove(entity);
                    await _context.SaveChangesAsync();

                    await _logService.LogEventAsync("Fac_02_Cuotas", "Elimina Registro", $"Elimina: Control_ID = '{entity.Control_ID}'", usuarioSigo, entity);
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Verifica si un registro de Fac_02_Cuotas existe por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <returns>Un valor que indica si el registro existe.</returns>
        public async Task<bool> ExistsByIdAsync(string id)
        {
            return await _context.Fac_02_Cuotas.AnyAsync(e => e.Control_ID == id);
        }

        /// <summary>
        /// Obtiene el tratamiento de una cuota por su ID.
        /// </summary>
        /// <param name="cuotaId">El ID de la cuota.</param>
        /// <returns>El tratamiento de la cuota.</returns>
        public async Task<string> GetTratamientoByCuotaIdAsync(string cuotaId)
        {
            return await _context.Dim_11_Cuota
                .Where(c => c.Cuota_ID == cuotaId)
                .Select(c => c.Tratamiento)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Obtiene el tipo de una cuota por su ID.
        /// </summary>
        /// <param name="cuotaId">El ID de la cuota.</param>
        /// <returns>El tipo de la cuota.</returns>
        public async Task<string> GetTipoCuotaByCuotaIdAsync(string cuotaId)
        {
            return await _context.Dim_11_Cuota
                .Where(c => c.Cuota_ID == cuotaId)
                .Select(c => c.TipoCuota)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Verifica si una cuota única existe para una especie y año.
        /// </summary>
        /// <param name="cuotaId">El ID de la cuota.</param>
        /// <param name="especieId">El ID de la especie.</param>
        /// <param name="año">El año de la cuota.</param>
        /// <returns>Un valor que indica si la cuota única existe.</returns>
        public async Task<bool> ExistsCuotaUnicaAsync(string cuotaId, string especieId, int año)
        {
            return await _context.Fac_02_Cuotas.AnyAsync(c =>
                c.Cuota_ID == cuotaId && c.Especie_ID == especieId && c.Año == año);
        }

        /// <summary>
        /// Obtiene la secuencia máxima de una cuota.
        /// </summary>
        /// <param name="cuotaId">El ID de la cuota.</param>
        /// <param name="especieId">El ID de la especie.</param>
        /// <param name="zonaId">El ID de la zona.</param>
        /// <param name="año">El año de la cuota.</param>
        /// <returns>La secuencia máxima de la cuota.</returns>
        public async Task<int> GetMaxSecuenciaAsync(string cuotaId, string especieId, string zonaId, int año)
        {
            return await _context.Fac_02_Cuotas
                .Where(c => c.Cuota_ID == cuotaId && c.Especie_ID == especieId && c.Zona_ID == zonaId && c.Año == año)
                .MaxAsync(c => (int?)c.Secuencia) ?? 0;
        }
    }
}