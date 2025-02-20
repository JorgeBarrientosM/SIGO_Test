using BackEnd.Data;
using BackEnd.Interfaces;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace BackEnd.Repositories
{
    public class Fac04Repository : IFac04Repository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogService _logService;

        public Fac04Repository(ApplicationDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        //---------------------------------------------------------------------
        // Métodos Transaccionales Principales
        //---------------------------------------------------------------------
        public async Task<string> RegistrarProduccionAsync(ProduccionRequest request, string usuarioSigo)
        {
            using var transaction = await BeginTransactionAsync();

            try
            {
                await ValidarCcoIdAsync(request.CcoId);
                await ValidarMareaAsync(request.MareaId);
                await ValidarProductosYLancesAsync(request.Lineas);

                var artePescaId = await ObtenerArtePescaIdPorCcoIdAsync(request.CcoId);
                var produccionId = await GenerarProduccionIdAsync(request.OperacionId, request.Lineas);

                var producciones = CrearEntidadesProduccionAsync(request, artePescaId, produccionId);
                await GuardarProduccionesAsync(await producciones, usuarioSigo);

                await transaction.CommitAsync();
                return produccionId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<string> ModificarProduccionAsync(ProduccionRequest request, string usuarioSigo)
        {
            using var transaction = await BeginTransactionAsync();

            try
            {
                var produccionOriginal = await ObtenerProduccionOriginalAsync(request.OperacionId);
                ValidarProduccionOriginal(produccionOriginal);

                produccionOriginal.Status = "M";
                await UpdateAsync(produccionOriginal, usuarioSigo);

                var nuevaProduccionId = await GenerarNuevaVersionProduccionIdAsync(request.OperacionId);
                var artePescaId = await ObtenerArtePescaIdPorCcoIdAsync(request.CcoId);

                var nuevasProducciones = CrearNuevasEntidadesProduccionAsync(request, artePescaId, nuevaProduccionId);
                await GuardarNuevasProduccionesAsync(await nuevasProducciones, usuarioSigo);

                await transaction.CommitAsync();
                return nuevaProduccionId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<string> IngresarCertificacionAsync(CertificacionRequest request, string usuarioSigo)
        {
            using var transaction = await BeginTransactionAsync();

            try
            {
                await ValidarMareaEnDescargaAsync(request.MareaId);
                await ValidarProduccionesPendientesCertificacionAsync(request.MareaId);

                var resumen = await CalcularResumenCertificacionAsync(request.MareaId, request.TotalCertificado);
                await GuardarResumenCertificacionAsync(resumen, usuarioSigo);

                await transaction.CommitAsync();
                return "Resumen generado correctamente.";
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<string> RegistrarDescargaAsync(DescargaRequest request, string usuarioSigo)
        {
            using var transaction = await BeginTransactionAsync();

            try
            {
                await ValidarMareaEnDescargaAsync(request.MareaId);
                await ValidarProduccionesParaDescargaAsync(request.MareaId);

                var descargas = CrearEntidadesDescarga(request);
                await GuardarDescargasAsync(descargas, usuarioSigo);

                await transaction.CommitAsync();
                return "Descarga registrada correctamente.";
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<string> RegistrarCertificacionFinalAsync(CertificaRequest request, string usuarioSigo)
        {
            using var transaction = await BeginTransactionAsync();

            try
            {
                await ValidarMareaEnDescargaAsync(request.MareaId);
                await ValidarProductoParaCertificacionAsync(request.ProductoId);

                var certificacion = CrearEntidadCertificacionAsync(request);
                await GuardarCertificacionAsync(await certificacion, usuarioSigo);

                await transaction.CommitAsync();
                return "Certificación registrada correctamente.";
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<string> CambiarEstadoMareaAsync(string mareaId, string usuarioSigo)
        {
            using var transaction = await BeginTransactionAsync();

            try
            {
                await ValidarMareaEnDescargaAsync(mareaId);
                await ValidarProduccionesParaCambioEstadoAsync(mareaId);

                await ActualizarEstadoMareaAsync(mareaId, "Certificada");
                await ActualizarCertificacionesAsync(mareaId);

                // Obtener las producciones actualizadas para registrar el evento
                var producciones = await ObtenerProduccionesPorMareaIdAsync(mareaId);
                foreach (var produccion in producciones)
                {
                    await _logService.LogEventAsync(
                        "Fac_04_Produccion",
                        "Actualiza Registro",
                        $"Actualiza: Produccion_ID = '{produccion.Produccion_ID}'",
                        usuarioSigo,
                        produccion
                    );
                }

                await transaction.CommitAsync();
                return "Estado de marea cambiado correctamente.";
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
        public async Task<List<Fac_04_Produccion>> GetAllAsync()
            => await _context.Fac_04_Produccion.ToListAsync();

        public async Task<Fac_04_Produccion> GetByIdAsync(string id)
            => await _context.Fac_04_Produccion.FindAsync(id);

        public async Task AddAsync(Fac_04_Produccion entity, string usuarioSigo)
        {
            _context.Fac_04_Produccion.Add(entity);
            await _context.SaveChangesAsync();
            await _logService.LogEventAsync(
                "Fac_04_Produccion",
                "Agrega Registro",
                $"Agrega: Produccion_ID = '{entity.Produccion_ID}'",
                usuarioSigo,
                entity
            );
        }

        public async Task UpdateAsync(Fac_04_Produccion entity, string usuarioSigo)
        {
            var existingEntity = await _context.Fac_04_Produccion.FindAsync(entity.Produccion_ID);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            await _logService.LogEventAsync(
                "Fac_04_Produccion",
                "Actualiza Registro",
                $"Actualiza: Produccion_ID = '{entity.Produccion_ID}'",
                usuarioSigo,
                entity
            );
        }

        public async Task DeleteAsync(string id, string usuarioSigo)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Fac_04_Produccion.Remove(entity);
                await _context.SaveChangesAsync();
                await _logService.LogEventAsync(
                    "Fac_04_Produccion",
                    "Elimina Registro",
                    $"Elimina: Produccion_ID = '{entity.Produccion_ID}'",
                    usuarioSigo,
                    entity
                );
            }
        }

        public async Task<bool> ExistsByIdAsync(string id)
            => await _context.Fac_04_Produccion.AnyAsync(e => e.Produccion_ID == id);

        //---------------------------------------------------------------------
        // Métodos de Validación
        //---------------------------------------------------------------------
        private async Task ValidarCcoIdAsync(string ccoId)
        {
            if (!await ExisteCcoIdAsync(ccoId))
                throw new ArgumentException("Cco_ID especificado no existe.");
        }

        private async Task ValidarMareaAsync(string mareaId)
        {
            var marea = await ObtenerMareaPorIdAsync(mareaId);
            if (marea == null)
                throw new ArgumentException("Marea_ID especificado no existe.");
            if (marea.EstadoOperativo != "en Curso")
                throw new ArgumentException("Marea especificada no está en curso.");
        }

        private async Task ValidarProductosYLancesAsync(List<ProduccionLinea> lineas)
        {
            foreach (var linea in lineas)
            {
                if (!await ExisteProductoIdAsync(linea.ProductoId))
                    throw new ArgumentException($"Producto_ID especificado no existe: {linea.ProductoId}.");
                if (linea.NroLance < 1)
                    throw new ArgumentException($"NroLance debe ser mayor o igual a 1 para el Producto_ID: {linea.ProductoId}.");
            }
        }

        private async Task ValidarMareaEnDescargaAsync(string mareaId)
        {
            var marea = await ObtenerMareaPorIdAsync(mareaId);
            if (marea == null || marea.EstadoOperativo != "en Descarga")
                throw new ArgumentException("La marea especificada no está en estado 'en Descarga'.");
        }

        private async Task ValidarProduccionesPendientesCertificacionAsync(string mareaId)
        {
            if (!await _context.Fac_04_Produccion.AnyAsync(p => p.Marea_ID == mareaId && p.Certificacion == "N" && p.Status == "A"))
                throw new ArgumentException("No se encontraron registros de producción pendientes de certificación para esta marea.");
        }

        private async Task ValidarProduccionesParaDescargaAsync(string mareaId)
        {
            if (!await _context.Fac_04_Produccion.AnyAsync(p => p.Marea_ID == mareaId && p.Status == "A"))
                throw new ArgumentException("No se encontraron registros de producción para la marea especificada.");
        }

        private async Task ValidarProductoParaCertificacionAsync(string productoId)
        {
            if (!await ExisteProductoIdAsync(productoId))
                throw new ArgumentException("Producto_ID especificado no existe.");
        }

        private async Task ValidarProduccionesParaCambioEstadoAsync(string mareaId)
        {
            if (!await _context.Fac_04_Produccion.AnyAsync(p => p.Marea_ID == mareaId))
                throw new ArgumentException("No se encontraron registros de producción para la marea especificada.");
        }

        //---------------------------------------------------------------------
        // Métodos de Soporte
        //---------------------------------------------------------------------
        private async Task<string> GenerarProduccionIdAsync(string operacionId, List<ProduccionLinea> lineas)
        {
            var produccionId = $"PR-{operacionId}-{lineas.First().ProductoId}-{lineas.First().NroLance:00}-V00";
            if (await ExisteProduccionIdAsync(produccionId))
                throw new ArgumentException("Produccion_ID ya existe.");
            return produccionId;
        }

        private async Task<string> GenerarNuevaVersionProduccionIdAsync(string operacionId)
        {
            var produccionIdBase = $"PR-{operacionId}";
            var nuevaVersion = await ObtenerNuevaVersionAsync(produccionIdBase) + 1;
            return $"{produccionIdBase}-V{nuevaVersion:00}";
        }

        private async Task<List<Fac_04_Produccion>> CrearEntidadesProduccionAsync(ProduccionRequest request, string artePescaId, string produccionId)
        {
            var producciones = new List<Fac_04_Produccion>();
            foreach (var linea in request.Lineas)
            {
                var precioUsdNullable = await ObtenerPrecioUsdAsync(linea.ProductoId, artePescaId);
                var factorNullable = await ObtenerFactorPorProductoIdAsync(linea.ProductoId);

                // Manejar valores nulos
                var precioUsd = precioUsdNullable ?? 0; // Si es null, usar 0 como valor predeterminado
                var factor = factorNullable ?? 0; // Si es null, usar 0 como valor predeterminado

                var biomasa = linea.Kilos * factor;

                producciones.Add(new Fac_04_Produccion
                {
                    Produccion_ID = $"{produccionId}-{linea.ProductoId}-{linea.NroLance:00}-V00",
                    Version = 0,
                    Operacion_ID = request.OperacionId,
                    Fecha = request.Fecha,
                    Cco_ID = request.CcoId,
                    Marea_ID = request.MareaId,
                    Producto_ID = linea.ProductoId,
                    Kilos = linea.Kilos,
                    Cajas = linea.Cajas,
                    PesoMedio = linea.Kilos / linea.Cajas,
                    Biomasa = biomasa,
                    PrecioUSD = precioUsd,
                    TotalUSD = linea.Kilos * precioUsd,
                    PrecioBio = biomasa > 0 ? (linea.Kilos * precioUsd) / biomasa : 0,
                    Sincronizado = "N",
                    NroLance = linea.NroLance,
                    Certificacion = "N",
                    Status = "A"
                });
            }
            return producciones;
        }

        private async Task<List<Fac_04_Produccion>> CrearNuevasEntidadesProduccionAsync(ProduccionRequest request, string artePescaId, string nuevaProduccionId)
        {
            var nuevasProducciones = new List<Fac_04_Produccion>();

            foreach (var linea in request.Lineas)
            {
                // Usar await para llamar a métodos asincrónicos
                var precioUsdNullable = await ObtenerPrecioUsdAsync(linea.ProductoId, artePescaId);
                var factorNullable = await ObtenerFactorPorProductoIdAsync(linea.ProductoId);

                // Manejar valores nulos
                var precioUsd = precioUsdNullable ?? 0; // Si es null, usar 0 como valor predeterminado
                var factor = factorNullable ?? 0; // Si es null, usar 0 como valor predeterminado

                var biomasa = linea.Kilos * factor;

                nuevasProducciones.Add(new Fac_04_Produccion
                {
                    Produccion_ID = $"{nuevaProduccionId}-{linea.ProductoId}-{linea.NroLance:00}-V00",
                    Version = 1,
                    Operacion_ID = request.OperacionId,
                    Fecha = request.Fecha,
                    Cco_ID = request.CcoId,
                    Marea_ID = request.MareaId,
                    Producto_ID = linea.ProductoId,
                    Kilos = linea.Kilos,
                    Cajas = linea.Cajas,
                    PesoMedio = linea.Kilos / linea.Cajas,
                    Biomasa = biomasa,
                    PrecioUSD = precioUsd,
                    TotalUSD = linea.Kilos * precioUsd,
                    PrecioBio = biomasa > 0 ? (linea.Kilos * precioUsd) / biomasa : 0,
                    Sincronizado = "N",
                    NroLance = linea.NroLance,
                    Certificacion = "N",
                    Status = "A"
                });
            }

            return nuevasProducciones;
        }

        private async Task GuardarProduccionesAsync(List<Fac_04_Produccion> producciones, string usuarioSigo)
        {
            foreach (var produccion in producciones)
            {
                await AddAsync(produccion, usuarioSigo);
            }
        }

        private async Task GuardarNuevasProduccionesAsync(List<Fac_04_Produccion> nuevasProducciones, string usuarioSigo)
        {
            foreach (var produccion in nuevasProducciones)
            {
                await AddAsync(produccion, usuarioSigo);
            }
        }

        private async Task<Fac_04_Produccion> ObtenerProduccionOriginalAsync(string operacionId)
        {
            return await _context.Fac_04_Produccion
                .Where(p => p.Operacion_ID == operacionId)
                .OrderByDescending(p => p.Version)
                .FirstOrDefaultAsync();
        }

        private void ValidarProduccionOriginal(Fac_04_Produccion produccionOriginal)
        {
            if (produccionOriginal == null)
                throw new ArgumentException("No se encontró la producción original.");
        }

        private async Task<List<Fac_04_Produccion>> CalcularResumenCertificacionAsync(string mareaId, decimal totalCertificado)
        {
            var producciones = await ObtenerProduccionesPorMareaIdAsync(mareaId);
            var totalKilos = await ObtenerTotalKilosPorMareaIdAsync(mareaId);
            var resumen = new List<Fac_04_Produccion>();

            if (totalKilos == 0)
            {
                throw new ArgumentException("No hay kilos registrados para esta marea.");
            }

            foreach (var produccion in producciones)
            {
                var porcentajeProduccion = produccion.Kilos / totalKilos;
                var kgsCertificados = porcentajeProduccion * totalCertificado;

                resumen.Add(new Fac_04_Produccion
                {
                    Produccion_ID = produccion.Produccion_ID,
                    Version = produccion.Version,
                    Operacion_ID = produccion.Operacion_ID,
                    Fecha = produccion.Fecha,
                    Cco_ID = produccion.Cco_ID,
                    Marea_ID = produccion.Marea_ID,
                    Producto_ID = produccion.Producto_ID,
                    Kilos = produccion.Kilos,
                    Cajas = produccion.Cajas,
                    PesoMedio = produccion.PesoMedio,
                    Biomasa = produccion.Biomasa,
                    PrecioUSD = produccion.PrecioUSD,
                    TotalUSD = produccion.TotalUSD,
                    PrecioBio = produccion.PrecioBio,
                    PorcentajeProduccion = porcentajeProduccion,
                    KgsCertificados = kgsCertificados,
                    Sincronizado = produccion.Sincronizado,
                    NroLance = produccion.NroLance,
                    Certificacion = produccion.Certificacion,
                    Status = produccion.Status
                });
            }

            return resumen;
        }

        private async Task GuardarResumenCertificacionAsync(List<Fac_04_Produccion> resumen, string usuarioSigo)
        {
            foreach (var produccion in resumen)
            {
                await UpdateAsync(produccion, usuarioSigo);
            }
        }

        private List<Fac_04_Produccion> CrearEntidadesDescarga(DescargaRequest request)
        {
            var descargas = new List<Fac_04_Produccion>();
            var producciones = _context.Fac_04_Produccion
                .Where(p => p.Marea_ID == request.MareaId && p.Certificacion == "N" && p.Status == "A")
                .GroupBy(p => p.Producto_ID)
                .Select(g => new Fac_04_Produccion
                {
                    Produccion_ID = $"DC-OP-{request.CcoId}-{request.FechaCertificacion:yyyyMMdd}-V99-{g.Key}-99-V99",
                    Version = 99,
                    Operacion_ID = $"OP-{request.CcoId}-{request.FechaCertificacion:yyyyMMdd}-V99",
                    Fecha = DateTime.Now,
                    Cco_ID = request.CcoId,
                    Marea_ID = request.MareaId,
                    Producto_ID = g.Key,
                    Kilos = g.Sum(p => p.Kilos) * -1,
                    Biomasa = g.Sum(p => p.Biomasa) * -1,
                    PrecioUSD = g.Sum(p => p.Kilos) > 0 ? g.Sum(p => p.TotalUSD) / g.Sum(p => p.Kilos) : 0,
                    TotalUSD = g.Sum(p => p.TotalUSD) * -1,
                    PrecioBio = g.Sum(p => p.Biomasa) > 0 ? g.Sum(p => p.TotalUSD) / g.Sum(p => p.Biomasa) : 0,
                    PorcentajeProduccion = 1,
                    KgsCertificados = g.Sum(p => p.KgsCertificados) * -1,
                    Sincronizado = "N",
                    NroLance = 99,
                    Certificacion = "N",
                    FechaCertificacion = request.FechaCertificacion,
                    NroCertificacion = request.NroCertificacion,
                    Cajas = g.Sum(p => p.Cajas) * -1,
                    PesoMedio = g.Sum(p => p.Cajas) != 0 ? g.Sum(p => p.Kilos) / g.Sum(p => p.Cajas) : 0,
                    Status = "A"
                })
                .ToList();

            descargas.AddRange(producciones);
            return descargas;
        }

        private async Task GuardarDescargasAsync(List<Fac_04_Produccion> descargas, string usuarioSigo)
        {
            foreach (var descarga in descargas)
            {
                await AddAsync(descarga, usuarioSigo);
            }
        }

        private async Task<Fac_04_Produccion> CrearEntidadCertificacionAsync(CertificaRequest request)
        {
            // Obtener ArtePesca_ID de manera asincrónica
            var artePescaId = await ObtenerArtePescaIdPorCcoIdAsync(request.CcoId);

            // Obtener PrecioUSD de manera asincrónica
            var precioUsdNullable = await ObtenerPrecioUsdAsync(request.ProductoId, artePescaId);
            var precioUsd = precioUsdNullable ?? 0; // Manejar valores nulos

            // Obtener Factor de manera asincrónica
            var factorNullable = await ObtenerFactorPorProductoIdAsync(request.ProductoId);
            var factor = factorNullable ?? 0; // Manejar valores nulos

            // Calcular Biomasa
            var biomasa = request.Kilos * factor;

            // Crear y retornar la entidad de producción
            return new Fac_04_Produccion
            {
                Produccion_ID = $"CF-{request.CcoId}-{request.FechaCertificacion:yyyyMMdd}-V99-{request.ProductoId}-99-V99",
                Version = 99,
                Operacion_ID = $"OP-{request.CcoId}-{request.FechaCertificacion:yyyyMMdd}-V99",
                Fecha = request.FechaCertificacion,
                Cco_ID = request.CcoId,
                Marea_ID = request.MareaId,
                Producto_ID = request.ProductoId,
                Kilos = request.Kilos,
                Biomasa = biomasa,
                PrecioUSD = precioUsd,
                TotalUSD = request.Kilos * precioUsd,
                PrecioBio = biomasa > 0 ? (request.Kilos * precioUsd) / biomasa : 0,
                PorcentajeProduccion = 1,
                KgsCertificados = request.Kilos,
                Sincronizado = "N",
                NroLance = 99,
                Certificacion = "N",
                FechaCertificacion = request.FechaCertificacion,
                NroCertificacion = request.NroCertificacion,
                Cajas = request.Cajas,
                PesoMedio = request.Kilos / request.Cajas,
                Status = "A"
            };
        }

        private async Task GuardarCertificacionAsync(Fac_04_Produccion certificacion, string usuarioSigo)
        {
            await AddAsync(certificacion, usuarioSigo);
        }

        private async Task ActualizarEstadoMareaAsync(string mareaId, string nuevoEstado)
        {
            var marea = await ObtenerMareaPorIdAsync(mareaId);
            marea.EstadoOperativo = nuevoEstado;
            marea.Estado = "I";
            _context.Fac_01_Mareas.Update(marea);
            await _context.SaveChangesAsync();
        }

        private async Task ActualizarCertificacionesAsync(string mareaId)
        {
            var producciones = await ObtenerProduccionesPorMareaIdAsync(mareaId);
            foreach (var produccion in producciones)
            {
                produccion.Certificacion = "S";
            }
            _context.Fac_04_Produccion.UpdateRange(producciones);
            await _context.SaveChangesAsync();
        }

        //---------------------------------------------------------------------
        // Métodos de Validación (Implementación)
        //---------------------------------------------------------------------
        public async Task<bool> ExisteCcoIdAsync(string ccoId)
            => await _context.Dim_01_Cco.AnyAsync(c => c.Cco_ID == ccoId);

        public async Task<Fac_01_Mareas> ObtenerMareaPorIdAsync(string mareaId)
            => await _context.Fac_01_Mareas.FirstOrDefaultAsync(m => m.Marea_ID == mareaId);

        public async Task<bool> ExisteProductoIdAsync(string productoId)
            => await _context.Dim_09_Productos.AnyAsync(p => p.Producto_ID == productoId);

        public async Task<string> ObtenerArtePescaIdPorCcoIdAsync(string ccoId)
            => await _context.Dim_03_Barcos
                .Where(b => b.Cco_ID == ccoId)
                .Select(b => b.ArtePesca_ID)
                .FirstOrDefaultAsync();

        public async Task<decimal?> ObtenerPrecioUsdAsync(string productoId, string artePescaId)
            => await _context.Dim_10_Precios
                .Where(p => p.Producto_ID == productoId && p.ArtePesca_ID == artePescaId && p.Estado == "A")
                .Select(p => p.PrecioUSD)
                .FirstOrDefaultAsync();

        public async Task<decimal?> ObtenerFactorPorProductoIdAsync(string productoId)
            => await _context.Dim_09_Productos
                .Where(p => p.Producto_ID == productoId)
                .Select(p => p.Factor)
                .FirstOrDefaultAsync();

        public async Task<bool> ExisteProduccionIdAsync(string produccionId)
            => await _context.Fac_04_Produccion.AnyAsync(p => p.Produccion_ID == produccionId);

        //---------------------------------------------------------------------
        // Métodos de Soporte (Implementación)
        //---------------------------------------------------------------------
        public async Task<int> ObtenerNuevaVersionAsync(string produccionIdBase)
            => await _context.Fac_04_Produccion
                .Where(p => p.Produccion_ID.StartsWith(produccionIdBase))
                .MaxAsync(p => (int?)p.Version) ?? 0;

        public async Task<List<Fac_04_Produccion>> ObtenerProduccionesPorMareaIdAsync(string mareaId)
            => await _context.Fac_04_Produccion
                .Where(p => p.Marea_ID == mareaId)
                .ToListAsync();

        public async Task<decimal> ObtenerTotalKilosPorMareaIdAsync(string mareaId)
            => await _context.Fac_04_Produccion
                .Where(p => p.Marea_ID == mareaId && p.Status == "A")
                .SumAsync(p => p.Kilos);

        public async Task<decimal> ObtenerTotalCajasPorMareaIdAsync(string mareaId)
            => await _context.Fac_04_Produccion
                .Where(p => p.Marea_ID == mareaId && p.Status == "A")
                .SumAsync(p => p.Cajas);

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