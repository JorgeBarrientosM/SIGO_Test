using BackEnd.Data;
using BackEnd.Interfaces;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace BackEnd.Repositories
{
    public class Fac02Repository : IFac02Repository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogService _logService;

        public Fac02Repository(ApplicationDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        //---------------------------------------------------------------------
        // Métodos CRUD Básicos
        //---------------------------------------------------------------------
        public async Task<List<Fac_02_Cuotas>> GetAllAsync()
            => await _context.Fac_02_Cuotas.ToListAsync();

        public async Task<Fac_02_Cuotas> GetByIdAsync(string id)
            => await _context.Fac_02_Cuotas.FindAsync(id);

        public async Task AddAsync(Fac_02_Cuotas entity, string usuarioSigo)
        {
            _context.Fac_02_Cuotas.Add(entity);
            await _context.SaveChangesAsync();

            await _logService.LogEventAsync(
                "Fac_02_Cuotas",
                "Agrega Registro",
                $"Agrega: Control_ID = '{entity.Control_ID}'",
                usuarioSigo,
                entity
            );
        }

        public async Task UpdateAsync(Fac_02_Cuotas entity, string usuarioSigo)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            await _logService.LogEventAsync(
                "Fac_02_Cuotas",
                "Actualiza Registro",
                $"Actualiza: Control_ID = '{entity.Control_ID}'",
                usuarioSigo,
                entity
            );
        }

        public async Task DeleteAsync(string id, string usuarioSigo)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Fac_02_Cuotas.Remove(entity);
                await _context.SaveChangesAsync();

                await _logService.LogEventAsync(
                    "Fac_02_Cuotas",
                    "Elimina Registro",
                    $"Elimina: Control_ID = '{entity.Control_ID}'",
                    usuarioSigo,
                    entity
                );
            }
        }

        public async Task<bool> ExistsByIdAsync(string id)
            => await _context.Fac_02_Cuotas.AnyAsync(e => e.Control_ID == id);

        //---------------------------------------------------------------------
        // Métodos de Validación
        //---------------------------------------------------------------------
        public async Task<string> GetTratamientoByCuotaIdAsync(string cuotaId)
            => await _context.Dim_11_Cuota
                .Where(c => c.Cuota_ID == cuotaId)
                .Select(c => c.Tratamiento)
                .FirstOrDefaultAsync();

        public async Task<string> GetTipoCuotaByCuotaIdAsync(string cuotaId)
            => await _context.Dim_11_Cuota
                .Where(c => c.Cuota_ID == cuotaId)
                .Select(c => c.TipoCuota)
                .FirstOrDefaultAsync();

        public async Task<bool> ExistsCuotaUnicaAsync(string cuotaId, string especieId, int año)
            => await _context.Fac_02_Cuotas.AnyAsync(c =>
                c.Cuota_ID == cuotaId && c.Especie_ID == especieId && c.Año == año);

        //---------------------------------------------------------------------
        // Métodos de Soporte
        //---------------------------------------------------------------------
        public async Task<int> GetMaxSecuenciaAsync(string cuotaId, string especieId, string zonaId, int año)
            => await _context.Fac_02_Cuotas
                .Where(c => c.Cuota_ID == cuotaId && c.Especie_ID == especieId && c.Zona_ID == zonaId && c.Año == año)
                .MaxAsync(c => (int?)c.Secuencia) ?? 0;

        public async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
            => await _context.Database.BeginTransactionAsync(isolationLevel);

        //---------------------------------------------------------------------
        // Métodos Transaccionales Principales
        //---------------------------------------------------------------------
        public async Task<string> AgregarCuotaAsync(AumentaCuotasRequest request, string usuarioSigo)
        {
            using var transaction = await BeginTransactionAsync();
            try
            {
                await ValidarCuotaParaAumentoAsync(request.Cuota_ID, request.Especie_ID, request.Año);

                var maxSecuencia = await GetMaxSecuenciaAsync(request.Cuota_ID, request.Especie_ID, request.Zona_ID, request.Año);
                var nuevaSecuencia = maxSecuencia + 1;

                var cuotaId = $"{request.Cuota_ID}-{request.Especie_ID}-{request.Zona_ID}-{request.Año}-{nuevaSecuencia:D3}";

                var nuevaCuota = new Fac_02_Cuotas
                {
                    Control_ID = cuotaId,
                    Cuota_ID = request.Cuota_ID,
                    Especie_ID = request.Especie_ID,
                    Año = request.Año,
                    Mes = request.Mes,
                    Toneladas = request.Toneladas,
                    Zona_ID = request.Zona_ID,
                    Secuencia = nuevaSecuencia,
                    Comentario = request.Comentario
                };

                await AddAsync(nuevaCuota, usuarioSigo);
                await transaction.CommitAsync();

                return cuotaId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<string> TraspasarCuotaAsync(RebajaCuotasRequest request, string usuarioSigo)
        {
            using var transaction = await BeginTransactionAsync();
            try
            {
                await ValidarCuotaParaRebajaAsync(request.Cuota_ID);

                var maxSecuencia = await GetMaxSecuenciaAsync(request.Cuota_ID, request.Especie_ID, request.Zona_ID, request.Año);
                var nuevaSecuencia = maxSecuencia + 1;

                var cuotaId = $"{request.Cuota_ID}-{request.Especie_ID}-{request.Zona_ID}-{request.Año}-{nuevaSecuencia:D3}";

                var nuevaCuota = new Fac_02_Cuotas
                {
                    Control_ID = cuotaId,
                    Cuota_ID = request.Cuota_ID,
                    Especie_ID = request.Especie_ID,
                    Año = request.Año,
                    Mes = request.Mes,
                    Toneladas = -request.Toneladas,
                    Zona_ID = request.Zona_ID,
                    Secuencia = nuevaSecuencia,
                    Comentario = request.Comentario
                };

                await AddAsync(nuevaCuota, usuarioSigo);
                await transaction.CommitAsync();

                return cuotaId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<string> CambiarZonaCuotaAsync(CambioZonaRequest request, string usuarioSigo)
        {
            using var transaction = await BeginTransactionAsync();
            try
            {
                await ValidarCuotaParaCambioZonaAsync(request.Cuota_ID);

                var maxSecuenciaOrigen = await GetMaxSecuenciaAsync(request.Cuota_ID, request.Especie_ID, request.ZonaOrigen_ID, request.Año);
                var nuevaSecuenciaOrigen = maxSecuenciaOrigen + 1;

                var cuotaIdOrigen = $"{request.Cuota_ID}-{request.Especie_ID}-{request.ZonaOrigen_ID}-{request.Año}-{nuevaSecuenciaOrigen:D3}";

                var cuotaOrigen = new Fac_02_Cuotas
                {
                    Control_ID = cuotaIdOrigen,
                    Cuota_ID = request.Cuota_ID,
                    Especie_ID = request.Especie_ID,
                    Año = request.Año,
                    Mes = request.Mes,
                    Toneladas = -request.Toneladas,
                    Zona_ID = request.ZonaOrigen_ID,
                    Secuencia = nuevaSecuenciaOrigen,
                    Comentario = request.Comentario
                };

                var maxSecuenciaDestino = await GetMaxSecuenciaAsync(request.Cuota_ID, request.Especie_ID, request.ZonaDestino_ID, request.Año);
                var nuevaSecuenciaDestino = maxSecuenciaDestino + 1;

                var cuotaIdDestino = $"{request.Cuota_ID}-{request.Especie_ID}-{request.ZonaDestino_ID}-{request.Año}-{nuevaSecuenciaDestino:D3}";

                var cuotaDestino = new Fac_02_Cuotas
                {
                    Control_ID = cuotaIdDestino,
                    Cuota_ID = request.Cuota_ID,
                    Especie_ID = request.Especie_ID,
                    Año = request.Año,
                    Mes = request.Mes,
                    Toneladas = request.Toneladas,
                    Zona_ID = request.ZonaDestino_ID,
                    Secuencia = nuevaSecuenciaDestino,
                    Comentario = request.Comentario
                };

                await AddAsync(cuotaOrigen, usuarioSigo);
                await AddAsync(cuotaDestino, usuarioSigo);
                await transaction.CommitAsync();

                return $"{cuotaIdOrigen},{cuotaIdDestino}";
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        //---------------------------------------------------------------------
        // Métodos de Validación (Implementación)
        //---------------------------------------------------------------------
        private async Task ValidarCuotaParaAumentoAsync(string cuotaId, string especieId, int año)
        {
            var tratamiento = await GetTratamientoByCuotaIdAsync(cuotaId);
            if (tratamiento != "Aumenta")
            {
                throw new ArgumentException("Cuota_ID especificado no corresponde a un movimiento de tipo Aumenta.");
            }

            var tipoCuota = await GetTipoCuotaByCuotaIdAsync(cuotaId);
            if (tipoCuota == "Unica" && await ExistsCuotaUnicaAsync(cuotaId, especieId, año))
            {
                throw new ArgumentException("Cuota Propia ya existe para esta Especie/Año, favor revisar y reintentar.");
            }
        }

        private async Task ValidarCuotaParaRebajaAsync(string cuotaId)
        {
            var tratamiento = await GetTratamientoByCuotaIdAsync(cuotaId);
            if (tratamiento != "Rebaja")
            {
                throw new ArgumentException("Cuota_ID especificado no corresponde a un movimiento de tipo Rebaja.");
            }
        }

        private async Task ValidarCuotaParaCambioZonaAsync(string cuotaId)
        {
            var tratamiento = await GetTratamientoByCuotaIdAsync(cuotaId);
            if (tratamiento != "Cambio Zona")
            {
                throw new ArgumentException("Cuota_ID especificado no corresponde a un movimiento de tipo Cambio Zona.");
            }
        }
    }
}