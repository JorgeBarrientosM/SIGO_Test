using BackEnd.Data;
using BackEnd.Interfaces;
using BackEnd.Models;
using BackEnd.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackEnd.Repositories
{
    /// <summary>
    /// Repositorio para la entidad Dim_99_Usuarios.
    /// </summary>
    public class Dim99Repository : IDim99Repository
    {
        private readonly ApplicationDbContext _context;
        private readonly IStateChangeService _stateChangeService;
        private readonly ILogService _logService;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Dim99Repository"/>.
        /// </summary>
        /// <param name="context">El contexto de la base de datos.</param>
        /// <param name="stateChangeService">El servicio de cambio de estado.</param>
        /// <param name="logService">El servicio de registro de eventos.</param>
        public Dim99Repository(ApplicationDbContext context, IStateChangeService stateChangeService, ILogService logService)
        {
            _context = context;
            _logService = logService;
            _stateChangeService = stateChangeService;
        }

        /// <summary>
        /// Obtiene todos los registros de Dim_99_Usuarios.
        /// </summary>
        /// <returns>Una lista de Dim_99_Usuarios.</returns>
        public async Task<IEnumerable<Dim_99_Usuarios>> GetAllAsync()
        {
            return await _context.Dim_99_Usuarios.ToListAsync();
        }

        /// <summary>
        /// Obtiene un registro de Dim_99_Usuarios por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <returns>El registro de Dim_99_Usuarios.</returns>
        public async Task<Dim_99_Usuarios> GetByIdAsync(string id)
        {
            return await _context.Dim_99_Usuarios.FindAsync(id);
        }

        /// <summary>
        /// Agrega un nuevo registro de Dim_99_Usuarios.
        /// </summary>
        /// <param name="entity">La entidad a agregar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        public async Task AddAsync(Dim_99_Usuarios entity, string usuarioSigo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Dim_99_Usuarios.Add(entity);
                await _context.SaveChangesAsync();
                await _logService.LogEventAsync("Dim_99_Usuarios", "Usuarios", $"Agrega: Usuario_ID = '{entity.Usuario_ID}'  //  Nombre = '{entity.NombreUsuario}'", usuarioSigo, "");
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Actualiza un registro existente de Dim_99_Usuarios.
        /// </summary>
        /// <param name="entity">La entidad a actualizar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        public async Task UpdateAsync(Dim_99_Usuarios entity, string usuarioSigo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                await _logService.LogEventAsync("Dim_99_Usuarios", "Usuarios", $"Actualiza: Usuario_ID = '{entity.Usuario_ID}'  //  Nombre = '{entity.NombreUsuario}'", usuarioSigo, "");
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Cambia el estado de un registro de Dim_99_Usuarios.
        /// </summary>
        /// <param name="entity">La entidad a actualizar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        public async Task ChangeStateAsync(Dim_99_Usuarios entity, string usuarioSigo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                await _logService.LogEventAsync("Dim_99_Usuarios", "Usuarios", $"Cambia Estado: Usuario_ID = '{entity.Usuario_ID}'  //  Nuevo Estado = '{entity.Estado}'", usuarioSigo, "");
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Restablece la contraseña de un registro de Dim_99_Usuarios.
        /// </summary>
        /// <param name="entity">La entidad a actualizar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        public async Task ResetPassAsync(Dim_99_Usuarios entity, string usuarioSigo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                await _logService.LogEventAsync("Dim_99_Usuarios", "Usuarios", $"Restablece Password: Usuario_ID = '{entity.Usuario_ID}'  //  Nombre = '{entity.NombreUsuario}'", usuarioSigo, "");
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Cambia la contraseña de un registro de Dim_99_Usuarios.
        /// </summary>
        /// <param name="entity">La entidad a actualizar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        public async Task ChangePassAsync(Dim_99_Usuarios entity, string usuarioSigo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                await _logService.LogEventAsync("Dim_99_Usuarios", "Usuarios", $"Actualiza Password: Usuario_ID = '{entity.Usuario_ID}'  //  Nombre = '{entity.NombreUsuario}'", usuarioSigo, "");
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Verifica si un registro de Dim_99_Usuarios existe por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <returns>Un valor que indica si el registro existe.</returns>
        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Dim_99_Usuarios.AnyAsync(e => e.Usuario_ID == id);
        }
    }
}