using BackEnd.Data;
using BackEnd.Interfaces;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BackEnd.Repositories
{
    /// <summary>
    /// Repositorio para la entidad DimXX.
    /// </summary>
    public class DimGenericRepository<T> : IDimGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly IStateChangeService _stateChangeService;
        private readonly ILogService _logService;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DimGenericRepository"/>.
        /// </summary>
        /// <param name="context">El contexto de la base de datos.</param>
        /// <param name="stateChangeService">El servicio de cambio de estado.</param>
        /// <param name="logService">El servicio de registro de eventos.</param>
        public DimGenericRepository(ApplicationDbContext context, IStateChangeService stateChangeService, ILogService logService)
        {
            _context = context;
            _logService = logService;
            _stateChangeService = stateChangeService;
        }

        /// <summary>
        /// Obtiene todos los registros de DimXX.
        /// </summary>
        /// <returns>Una lista de DimXX.</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un registro de DimXX por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <returns>El registro de DimXX.</returns>
        public async Task<T> GetByIdAsync(string id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// Agrega un nuevo registro de DimXX.
        /// </summary>
        /// <param name="entity">La entidad a agregar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        public async Task AddAsync(T entity, string usuarioSigo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var entityType = _context.Model.FindEntityType(typeof(T));
            var primaryKey = entityType.FindPrimaryKey();
            var keyValues = primaryKey.Properties.Select(p => p.PropertyInfo.GetValue(entity)?.ToString());
            string idValue = string.Join(",", keyValues) ?? "Desconocido";

            var keyName = primaryKey.Properties.Select(p => p.Name).FirstOrDefault() ?? "Desconocido";

            try
            {
                _context.Set<T>().Add(entity);
                await _context.SaveChangesAsync();
                await _logService.LogEventAsync(typeof(T).Name, "Agrega Registro", $"Agrega: {keyName} = '{idValue}'", usuarioSigo, entity);
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Actualiza un registro existente de DimXX.
        /// </summary>
        /// <param name="entity">La entidad a actualizar.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        public async Task UpdateAsync(T entity, string usuarioSigo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var entityType = _context.Model.FindEntityType(typeof(T));
            var primaryKey = entityType.FindPrimaryKey();
            var keyValues = primaryKey.Properties.Select(p => p.PropertyInfo.GetValue(entity)?.ToString());
            string idValue = string.Join(",", keyValues) ?? "Desconocido";

            var keyName = primaryKey.Properties.Select(p => p.Name).FirstOrDefault() ?? "Desconocido";

            try
            {
                _context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();
                await _logService.LogEventAsync(typeof(T).Name, "Actualiza Registro", $"Actualiza: {keyName} = '{idValue}'", usuarioSigo, entity);
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Cambia Estado a un registro de DimXX por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <param name="usuarioSigo">El usuario que realiza la acción.</param>
        public async Task DeleteAsync(string id, string usuarioSigo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entity = await _context.Set<T>().FindAsync(id);
                if (entity != null)
                {
                    await _stateChangeService.ChangeStateAsync(entity, usuarioSigo);
                    await _context.SaveChangesAsync();
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
        /// Verifica si un registro de DimXX existe por su ID.
        /// </summary>
        /// <param name="id">El ID del registro.</param>
        /// <returns>Un valor que indica si el registro existe.</returns>
        public async Task<bool> ExistsAsync(string id)
        {
            var entityType = _context.Model.FindEntityType(typeof(T));
            var primaryKey = entityType.FindPrimaryKey();
            var keyProperties = primaryKey.Properties.Select(p => p.Name).ToList();

            // Construir la expresión dinámica para verificar la existencia
            var parameter = Expression.Parameter(typeof(T), "e");
            var keyProperty = typeof(T).GetProperty(keyProperties[0]); // Asume una sola clave primaria
            var propertyAccess = Expression.Property(parameter, keyProperty);
            var equalsExpression = Expression.Equal(propertyAccess, Expression.Constant(id));
            var lambda = Expression.Lambda<Func<T, bool>>(equalsExpression, parameter);

            return await _context.Set<T>().AnyAsync(lambda);
        }
    }
}