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
            // Validaciones
            if (!await ExistsCcoIdAsync(request.CcoId))
                throw new ArgumentException("Cco_ID especificado no existe.");

            var marea = await GetMareaByIdAsync(request.MareaId);
            if (marea == null)
                throw new ArgumentException("Marea_ID especificado no existe.");
            if (marea.EstadoOperativo != "en Curso")
                throw new ArgumentException("Marea especificada no está en curso.");

            if (request.OperatividadId == "OP" && (request.Lances == null || request.Lances <= 0))
                throw new ArgumentException("Lances debe ser > 0 cuando Operatividad_ID es 'OP'.");
            else if (request.OperatividadId != "OP" && request.Lances != 0)
                throw new ArgumentException("Lances debe ser 0 cuando OperatividadId no es 'OP'.");

            // Generar Operacion_ID
            var operacionId = $"OP-{request.CcoId}-{request.Fecha:yyyyMMdd}-V00";
            if (await ExistsByIdAsync(operacionId))
                throw new ArgumentException("Operacion_ID ya existe.");

            // Calcular PiezasCarnada
            var piezasCarnada = 0;
            if (request.CarnadaId != null)
            {
                var carnada = await GetCarnadaByIdAsync(request.CarnadaId);
                piezasCarnada = (int)(request.ConsumoCarnada * carnada.PiezasKg);
            }

            // Crear entidad
            var operacion = new Fac_03_Operacion
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
                HrsTotal = 24, // Validado en controlador
                Sincronizado = "N",
                Status = "A"
            };

            await AddAsync(operacion, usuarioSigo); // Usa el método CRUD básico
            return operacion.Operacion_ID;
        }

        public async Task<string> ModificarOperacionAsync(OperacionRequest request, string usuarioSigo)
        {
            var operacionOriginal = await GetOperacionByCcoIdAndFechaAsync(request.CcoId, request.Fecha);
            if (operacionOriginal == null)
                throw new ArgumentException("No se encontró la operación OP original.");

            operacionOriginal.Status = "M";
            await UpdateAsync(operacionOriginal, usuarioSigo);

            // Generar nueva versión
            var operacionIdBase = $"OP-{request.CcoId}-{request.Fecha:yyyyMMdd}";
            var nuevaVersion = await GetMaxVersionByOperacionIdBaseAsync(operacionIdBase) + 1;
            var nuevaOperacionId = $"{operacionIdBase}-V{nuevaVersion:00}";

            var nuevaOperacion = new Fac_03_Operacion
            {
                Operacion_ID = nuevaOperacionId,
                Version = nuevaVersion,
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
                PiezasCarnada = await CalcularPiezasCarnadaAsync(request.CarnadaId, request.ConsumoCarnada),
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

            await AddAsync(nuevaOperacion, usuarioSigo);
            return nuevaOperacion.Operacion_ID;
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
        // Métodos de Soporte
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

        //---------------------------------------------------------------------
        // Métodos Privados
        //---------------------------------------------------------------------
        private async Task<int> CalcularPiezasCarnadaAsync(string carnadaId, decimal? consumoCarnada)
        {
            if (carnadaId == null || consumoCarnada == null) return 0;
            var carnada = await GetCarnadaByIdAsync(carnadaId);
            return (int)(consumoCarnada * carnada.PiezasKg);
        }
    }
}