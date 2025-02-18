using BackEnd.Data;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace BackEnd.Interfaces
{
    public interface IFac03Repository
    {
        // Métodos Transaccionales Principales
        Task<string> RegistrarOperacionAsync(OperacionRequest request, string usuarioSigo);
        Task<string> ModificarOperacionAsync(OperacionRequest request, string usuarioSigo);

        // Métodos CRUD Básicos
        Task<List<Fac_03_Operacion>> GetAllAsync();
        Task<Fac_03_Operacion> GetByIdAsync(string id);
        Task AddAsync(Fac_03_Operacion entity, string usuarioSigo);
        Task UpdateAsync(Fac_03_Operacion entity, string usuarioSigo);
        Task DeleteAsync(string id, string usuarioSigo);
        Task<bool> ExistsByIdAsync(string id);

        // Métodos de Validación
        Task<bool> ExistsCcoIdAsync(string ccoId);
        Task<Fac_01_Mareas> GetMareaByIdAsync(string mareaId);
        Task<bool> ExistsOperatividadIdAsync(string operatividadId);
        Task<bool> ExistsCarnadaIdAsync(string carnadaId);
        Task<Dim_13_Carnada> GetCarnadaByIdAsync(string carnadaId);

        // Métodos de Soporte
        Task<Fac_03_Operacion> GetOperacionByCcoIdAndFechaAsync(string ccoId, DateTime fecha);
        Task<int> GetMaxVersionByOperacionIdBaseAsync(string operacionIdBase);
        Task AddSyncLogAsync(Fac_99_Sync log);
        Task AddLogErrorAsync(Fac_98_Log logError);
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}