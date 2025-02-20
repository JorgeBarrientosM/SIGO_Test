using BackEnd.Data;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace BackEnd.Interfaces
{
    public interface IFac04Repository
    {
        // Métodos Transaccionales Principales
        Task<string> RegistrarProduccionAsync(ProduccionRequest request, string usuarioSigo);
        Task<string> ModificarProduccionAsync(ProduccionRequest request, string usuarioSigo);
        Task<string> IngresarCertificacionAsync(CertificacionRequest request, string usuarioSigo);
        Task<string> RegistrarDescargaAsync(DescargaRequest request, string usuarioSigo);
        Task<string> RegistrarCertificacionFinalAsync(CertificaRequest request, string usuarioSigo);
        Task<string> CambiarEstadoMareaAsync(string mareaId, string usuarioSigo);

        // Métodos CRUD Básicos
        Task<List<Fac_04_Produccion>> GetAllAsync();
        Task<Fac_04_Produccion> GetByIdAsync(string id);
        Task AddAsync(Fac_04_Produccion entity, string usuarioSigo);
        Task UpdateAsync(Fac_04_Produccion entity, string usuarioSigo);
        Task DeleteAsync(string id, string usuarioSigo);
        Task<bool> ExistsByIdAsync(string id);

        // Métodos de Validación
        Task<bool> ExisteCcoIdAsync(string ccoId);
        Task<Fac_01_Mareas> ObtenerMareaPorIdAsync(string mareaId);
        Task<bool> ExisteProductoIdAsync(string productoId);
        Task<string> ObtenerArtePescaIdPorCcoIdAsync(string ccoId);
        Task<decimal?> ObtenerPrecioUsdAsync(string productoId, string artePescaId);
        Task<decimal?> ObtenerFactorPorProductoIdAsync(string productoId);
        Task<bool> ExisteProduccionIdAsync(string produccionId);

        // Métodos de Soporte
        Task<int> ObtenerNuevaVersionAsync(string produccionIdBase);
        Task<List<Fac_04_Produccion>> ObtenerProduccionesPorMareaIdAsync(string mareaId);
        Task<decimal> ObtenerTotalKilosPorMareaIdAsync(string mareaId);
        Task<decimal> ObtenerTotalCajasPorMareaIdAsync(string mareaId);
        Task AddSyncLogAsync(Fac_99_Sync log);
        Task AddLogErrorAsync(Fac_98_Log logError);
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}