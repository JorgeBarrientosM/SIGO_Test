using BackEnd.Data;
using BackEnd.Interfaces;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace BackEnd.Repositories
{
    public class Fac03Repository : IFac03Repository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogService _logService;

        public Fac03Repository(ApplicationDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        //---------------------------------------------------------------------
        // Métodos Transaccionales Principales (Nuevos)                        
        //---------------------------------------------------------------------
        public async Task<string> RegistrarOperacionAsync(OperacionRequest request, string usuarioSigo)
        {
            using var transaction = await BeginTransactionAsync();

            try
            {
                await ValidarCcoIdAsync(request.CcoId);
                await ValidarMareaAsync(request.MareaId);
                await ValidarOperatividadYLancesAsync(request.OperatividadId, request.Lances);

                var operacionId = await GenerarOperacionIdAsync(request.CcoId, request.Fecha);
                var piezasCarnada = await CalcularPiezasCarnadaAsync(request.CarnadaId, request.ConsumoCarnada);

                var operacion = CrearEntidadOperacion(request, operacionId, piezasCarnada);
                await GuardarOperacionAsync(operacion, usuarioSigo);

                await transaction.CommitAsync();
                return operacion.Operacion_ID;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<string> ModificarOperacionAsync(OperacionRequest request, string usuarioSigo)
        {
            using var transaction = await BeginTransactionAsync();

            try
            {
                var operacionOriginal = await ObtenerOperacionOriginalAsync(request.CcoId, request.Fecha);
                ValidarOperacionOriginal(operacionOriginal);

                operacionOriginal.Status = "M";
                await UpdateAsync(operacionOriginal, usuarioSigo);

                var nuevaOperacionId = await GenerarNuevaVersionOperacionIdAsync(request.CcoId, request.Fecha);
                var piezasCarnada = await CalcularPiezasCarnadaAsync(request.CarnadaId, request.ConsumoCarnada);

                var nuevaOperacion = CrearNuevaEntidadOperacion(request, operacionOriginal, nuevaOperacionId, piezasCarnada);
                await GuardarNuevaOperacionAsync(nuevaOperacion, usuarioSigo);

                await transaction.CommitAsync();
                return nuevaOperacion.Operacion_ID;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        //---------------------------------------------------------------------
        // Métodos CRUD Básicos
        //---------------------------------------------------------------------
        public async Task<List<Fac_03_Operacion>> GetAllAsync()
            => await _context.Fac_03_Operacion.ToListAsync();

        public async Task<Fac_03_Operacion> GetByIdAsync(string id)
            => await _context.Fac_03_Operacion.FindAsync(id);

        public async Task AddAsync(Fac_03_Operacion entity, string usuarioSigo)
        {
            _context.Fac_03_Operacion.Add(entity);
            await _context.SaveChangesAsync();
            await _logService.LogEventAsync(
                "Fac_03_Operacion",
                "Agrega Registro",
                $"Agrega: Operacion_ID = '{entity.Operacion_ID}'",
                usuarioSigo,
                entity
            );
        }

        public async Task UpdateAsync(Fac_03_Operacion entity, string usuarioSigo)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            await _logService.LogEventAsync(
                "Fac_03_Operacion",
                "Actualiza Registro",
                $"Actualiza: Operacion_ID = '{entity.Operacion_ID}'",
                usuarioSigo,
                entity
            );
        }

        public async Task DeleteAsync(string id, string usuarioSigo)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Fac_03_Operacion.Remove(entity);
                await _context.SaveChangesAsync();
                await _logService.LogEventAsync(
                    "Fac_03_Operacion",
                    "Elimina Registro",
                    $"Elimina: Operacion_ID = '{entity.Operacion_ID}'",
                    usuarioSigo,
                    entity
                );
            }
        }

        public async Task<bool> ExistsByIdAsync(string id)
            => await _context.Fac_03_Operacion.AnyAsync(e => e.Operacion_ID == id);

        //---------------------------------------------------------------------
        // Métodos de Validación
        //---------------------------------------------------------------------
        private async Task ValidarCcoIdAsync(string ccoId)
        {
            if (!await ExistsCcoIdAsync(ccoId))
                throw new ArgumentException("Cco_ID especificado no existe.");
        }

        private async Task ValidarMareaAsync(string mareaId)
        {
            var marea = await GetMareaByIdAsync(mareaId);
            if (marea == null)
                throw new ArgumentException("Marea_ID especificado no existe.");
            if (marea.EstadoOperativo != "en Curso")
                throw new ArgumentException("Marea especificada no está en curso.");
        }

        private async Task ValidarOperatividadYLancesAsync(string operatividadId, int? lances)
        {
            if (operatividadId == "OP" && (lances == null || lances <= 0))
                throw new ArgumentException("Lances debe ser > 0 cuando Operatividad_ID es 'OP'.");
            else if (operatividadId != "OP" && lances != 0)
                throw new ArgumentException("Lances debe ser 0 cuando OperatividadId no es 'OP'.");
        }

        //---------------------------------------------------------------------
        // Métodos de Soporte
        //---------------------------------------------------------------------
        private async Task<string> GenerarOperacionIdAsync(string ccoId, DateTime fecha)
        {
            var operacionId = $"OP-{ccoId}-{fecha:yyyyMMdd}-V00";
            if (await ExistsByIdAsync(operacionId))
                throw new ArgumentException("Operacion_ID ya existe.");
            return operacionId;
        }

        private async Task<string> GenerarNuevaVersionOperacionIdAsync(string ccoId, DateTime fecha)
        {
            var operacionIdBase = $"OP-{ccoId}-{fecha:yyyyMMdd}";
            var nuevaVersion = await GetMaxVersionByOperacionIdBaseAsync(operacionIdBase) + 1;
            return $"{operacionIdBase}-V{nuevaVersion:00}";
        }

        private Fac_03_Operacion CrearEntidadOperacion(OperacionRequest request, string operacionId, int piezasCarnada)
        {
            return new Fac_03_Operacion
            {
                Operacion_ID = operacionId,
                Version = 0,
                Cco_ID = request.CcoId,
                Fecha = request.Fecha,
                Marea_ID = request.MareaId,
                Operatividad_ID = request.OperatividadId,
                MillasNauticas = request.MillasNauticas ?? 0,
                FuerzaViento = request.FuerzaViento ?? 0,
                Latitud = request.Latitud ?? 0,
                Longitud = request.Longitud ?? 0,
                Lances = request.Lances ?? 0,
                ConsumoPetroleo = request.ConsumoPetroleo ?? 0,
                Carnada_ID = request.CarnadaId,
                ConsumoCarnada = request.ConsumoCarnada ?? 0,
                PiezasCarnada = piezasCarnada,
                Anzuelos = request.Anzuelos ?? 0,
                HrsPesca = request.HrsPesca ?? 0,
                HrsRuta = request.HrsRuta ?? 0,
                HrsCapa = request.HrsCapa ?? 0,
                HrsAveria = request.HrsAveria ?? 0,
                HrsAccidente = request.HrsAccidente ?? 0,
                HrsPersonal = request.HrsPersonal ?? 0,
                HrsTotal = 24,
                Sincronizado = "N",
                Status = "A"
            };
        }

        private Fac_03_Operacion CrearNuevaEntidadOperacion(OperacionRequest request, Fac_03_Operacion operacionOriginal, string nuevaOperacionId, int piezasCarnada)
        {
            return new Fac_03_Operacion
            {
                Operacion_ID = nuevaOperacionId,
                Version = operacionOriginal.Version + 1,
                Cco_ID = operacionOriginal.Cco_ID,
                Fecha = operacionOriginal.Fecha,
                Marea_ID = operacionOriginal.Marea_ID,
                Operatividad_ID = request.OperatividadId,
                MillasNauticas = request.MillasNauticas ?? 0,
                FuerzaViento = request.FuerzaViento ?? 0,
                Latitud = request.Latitud ?? 0,
                Longitud = request.Longitud ?? 0,
                Lances = request.Lances ?? 0,
                ConsumoPetroleo = request.ConsumoPetroleo ?? 0,
                Carnada_ID = request.CarnadaId,
                ConsumoCarnada = request.ConsumoCarnada ?? 0,
                PiezasCarnada = piezasCarnada,
                Anzuelos = request.Anzuelos ?? 0,
                HrsPesca = request.HrsPesca ?? 0,
                HrsRuta = request.HrsRuta ?? 0,
                HrsCapa = request.HrsCapa ?? 0,
                HrsAveria = request.HrsAveria ?? 0,
                HrsAccidente = request.HrsAccidente ?? 0,
                HrsPersonal = request.HrsPersonal ?? 0,
                HrsTotal = 24,
                Sincronizado = "N",
                Status = "A"
            };
        }

        private async Task GuardarOperacionAsync(Fac_03_Operacion operacion, string usuarioSigo)
        {
            await AddAsync(operacion, usuarioSigo);
        }

        private async Task GuardarNuevaOperacionAsync(Fac_03_Operacion nuevaOperacion, string usuarioSigo)
        {
            await AddAsync(nuevaOperacion, usuarioSigo);
        }

        private async Task<Fac_03_Operacion> ObtenerOperacionOriginalAsync(string ccoId, DateTime fecha)
        {
            return await GetOperacionByCcoIdAndFechaAsync(ccoId, fecha);
        }

        private void ValidarOperacionOriginal(Fac_03_Operacion operacionOriginal)
        {
            if (operacionOriginal == null)
                throw new ArgumentException("No se encontró la operación OP original.");
        }

        private async Task<int> CalcularPiezasCarnadaAsync(string carnadaId, decimal? consumoCarnada)
        {
            if (carnadaId == null || consumoCarnada == null) return 0;
            var carnada = await GetCarnadaByIdAsync(carnadaId);
            return (int)(consumoCarnada * carnada.PiezasKg);
        }

        //---------------------------------------------------------------------
        // Métodos de Validación (Implementación)
        //---------------------------------------------------------------------
        public async Task<bool> ExistsCcoIdAsync(string ccoId)
            => await _context.Dim_01_Cco.AnyAsync(c => c.Cco_ID == ccoId);

        public async Task<Fac_01_Mareas> GetMareaByIdAsync(string mareaId)
            => await _context.Fac_01_Mareas.FirstOrDefaultAsync(m => m.Marea_ID == mareaId);

        public async Task<bool> ExistsOperatividadIdAsync(string operatividadId)
            => await _context.Dim_04_Operatividad.AnyAsync(o => o.Operatividad_ID == operatividadId);

        public async Task<bool> ExistsCarnadaIdAsync(string carnadaId)
            => await _context.Dim_13_Carnada.AnyAsync(c => c.Carnada_ID == carnadaId);

        public async Task<Dim_13_Carnada> GetCarnadaByIdAsync(string carnadaId)
            => await _context.Dim_13_Carnada.FirstOrDefaultAsync(c => c.Carnada_ID == carnadaId);

        //---------------------------------------------------------------------
        // Métodos de Soporte (Implementación)
        //---------------------------------------------------------------------
        public async Task<Fac_03_Operacion> GetOperacionByCcoIdAndFechaAsync(string ccoId, DateTime fecha)
            => await _context.Fac_03_Operacion
                .Where(o => o.Cco_ID == ccoId && o.Fecha == fecha)
                .OrderByDescending(o => o.Version)
                .FirstOrDefaultAsync();

        public async Task<int> GetMaxVersionByOperacionIdBaseAsync(string operacionIdBase)
            => await _context.Fac_03_Operacion
                .Where(o => o.Operacion_ID.StartsWith(operacionIdBase))
                .MaxAsync(o => (int?)o.Version) ?? 0;

        public async Task AddSyncLogAsync(Fac_99_Sync log)
        {
            _context.Fac_99_Sync.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task AddLogErrorAsync(Fac_98_Log logError)
        {
            _context.Fac_98_Log.Add(logError);
            await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
            => await _context.Database.BeginTransactionAsync(isolationLevel);
    }
}