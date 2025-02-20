using System.Data;
using BackEnd.Data;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore.Storage;

public interface IFac02Repository
{
    // Métodos CRUD Básicos
    Task<List<Fac_02_Cuotas>> GetAllAsync();
    Task<Fac_02_Cuotas> GetByIdAsync(string id);
    Task AddAsync(Fac_02_Cuotas entity, string usuarioSigo);
    Task UpdateAsync(Fac_02_Cuotas entity, string usuarioSigo);
    Task DeleteAsync(string id, string usuarioSigo);
    Task<bool> ExistsByIdAsync(string id);

    // Métodos de Validación
    Task<string> GetTratamientoByCuotaIdAsync(string cuotaId);
    Task<string> GetTipoCuotaByCuotaIdAsync(string cuotaId);
    Task<bool> ExistsCuotaUnicaAsync(string cuotaId, string especieId, int año);

    // Métodos de Soporte
    Task<int> GetMaxSecuenciaAsync(string cuotaId, string especieId, string zonaId, int año);
    Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

    // Métodos para operaciones específicas
    Task<string> AgregarCuotaAsync(AumentaCuotasRequest request, string usuarioSigo);
    Task<string> TraspasarCuotaAsync(RebajaCuotasRequest request, string usuarioSigo);
    Task<string> CambiarZonaCuotaAsync(CambioZonaRequest request, string usuarioSigo);
}