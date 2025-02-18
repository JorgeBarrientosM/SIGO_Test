using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEnd.Data;
using BackEnd.Models;
using System.Data.SqlTypes;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using BackEnd.Filters;

namespace BackEnd.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ValidateUserFilter))]
    public class Fac04ProduccionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Fac04ProduccionController> _logger;

        public Fac04ProduccionController(ApplicationDbContext context, ILogger<Fac04ProduccionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Registra nueva Producción de Parte de Pesca diario (Barco).
        /// </summary>
        /// <param name="request">Los detalles de la producción a registrar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("ingresa-produccion-pp")]
        public async Task<IActionResult> RegistrarProduccion([FromBody] ProduccionRequest request, [FromQuery] string usuarioSigo)
        {
            // Obtener la fecha y hora del sistema
            var fechaActual = DateTime.Now.Date;
            var horaActual = DateTime.Now.TimeOfDay;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validar existencia de Cco_ID y Marea_ID
                if (!await _context.Dim_01_Cco.AnyAsync(c => c.Cco_ID == request.CcoId))
                {
                    return BadRequest(new { codigoEstado = 0, mensaje = "Cco_ID especificado no existe." });
                }

                var marea = await _context.Fac_01_Mareas.FirstOrDefaultAsync(m => m.Marea_ID == request.MareaId);
                if (marea == null)
                {
                    return BadRequest(new { codigoEstado = 0, mensaje = "Marea_ID especificado no existe." });
                }

                if (marea.EstadoOperativo != "en Curso")
                {
                    return BadRequest(new { codigoEstado = 0, mensaje = "Marea especificada no está en curso." });
                }

                // Validar existencia de Producto_ID y NroLance en cada línea
                foreach (var linea in request.Lineas)
                {
                    if (!await _context.Dim_09_Productos.AnyAsync(p => p.Producto_ID == linea.ProductoId))
                    {
                        return BadRequest(new { codigoEstado = 0, mensaje = $"Producto_ID especificado no existe: {linea.ProductoId}." });
                    }

                    if (linea.NroLance < 1)
                    {
                        return BadRequest(new { codigoEstado = 0, mensaje = $"NroLance debe ser mayor o igual a 1 para el Producto_ID: {linea.ProductoId}." });
                    }
                }

                // Obtener ArtePesca_ID desde Dim_03_Barcos
                var artePescaId = await _context.Dim_03_Barcos
                    .Where(b => b.Cco_ID == request.CcoId)
                    .Select(b => b.ArtePesca_ID)
                    .FirstOrDefaultAsync();

                if (artePescaId == null)
                {
                    return BadRequest(new
                    { codigoEstado = 0, mensaje = "No se encontró ArtePesca_ID asociado al Cco_ID especificado." });
                }

                // Procesar cada línea de producción
                foreach (var linea in request.Lineas)
                {
                    var precioUsd = await _context.Dim_10_Precios
                        .Where(p => p.Producto_ID == linea.ProductoId && p.ArtePesca_ID == artePescaId && p.Estado == "A")
                        .Select(p => p.PrecioUSD)
                        .FirstOrDefaultAsync();

                    if (precioUsd == null)
                    {
                        return BadRequest(new
                        {
                            codigoEstado = 0,
                            mensaje = $"No se encontró un precio activo para el Producto_ID: {linea.ProductoId} y ArtePesca_ID especificados."
                        });
                    }

                    // Obtener el Factor desde Dim_09_Productos
                    var factor = await _context.Dim_09_Productos
                        .Where(p => p.Producto_ID == linea.ProductoId)
                        .Select(p => p.Factor)
                        .FirstOrDefaultAsync();

                    if (factor == null)
                    {
                        return BadRequest(new
                        {
                            codigoEstado = 0,
                            mensaje = $"No se encontró un Factor para el Producto_ID: {linea.ProductoId} especificado."
                        });
                    }

                    // Calcular Biomasa
                    var biomasa = linea.Kilos * factor;

                    // Generar Produccion_ID en formato 
                    var produccionId = $"PR-{request.OperacionId}-{linea.ProductoId}-{linea.NroLance:00}-V00";

                    // Verificar si ya existe un registro con el mismo Produccion_ID
                    if (await _context.Fac_04_Produccion.AnyAsync(p => p.Produccion_ID == produccionId))
                    {
                        return BadRequest(new { codigoEstado = 0, mensaje = $"Produccion_ID especificado ya existe: {produccionId}." });
                    }

                    // Crear nueva producción
                    var nuevaProduccion = new Fac_04_Produccion
                    {
                        Produccion_ID = produccionId,
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
                        Status= "A"
                    };

                    // Inserción
                    _context.Fac_04_Produccion.Add(nuevaProduccion);
                }

                await _context.SaveChangesAsync();

                // Registro en Fac_99_Sync
                var log = new Fac_99_Sync
                {
                    Orden = $"DATA-PR-{request.Fecha:yyyyMMdd}-{request.CcoId}-V00",
                    Fecha = fechaActual,
                    Hora = horaActual,
                    TipoEvento = "Parte Pesca",
                    Origen = "Barco",
                    Detalle = $"Producción registrada: Operacion_ID = {request.OperacionId}",
                    Usuario_ID = usuarioSigo
                };

                _context.Fac_99_Sync.Add(log);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    codigoEstado = 1,
                    mensaje = "Inserción exitosa."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // Manejo de errores generales
                var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _logger.LogError(ex, "Error general.");

                // Log adicional para verificar el registro de errores
                _logger.LogInformation("Registrando error en Fac_98_Log");

                // Registro de error en Fac_98_Log
                var logError = new Fac_98_Log
                {
                    FechaHora = DateTime.Now,
                    Modulo = "Fac_04_Produccion",
                    Evento = "Error",
                    Detalle = $"Error al ingresar Producción: {ex.Message}",
                    Usuario_ID = usuarioSigo
                };
                _context.Fac_98_Log.Add(logError);
                await _context.SaveChangesAsync();

                // Manejo específico de errores de valores nulos
                if (ex is SqlNullValueException)
                {
                    return StatusCode(500,
                        new
                        {
                            codigoEstado = 0,
                            mensaje = "Error de valor nulo en la base de datos.",
                            detalle = ex.StackTrace
                        });
                }

                // Verificar si es un error de duplicación de clave primaria
                if (ex is DbUpdateException dbUpdateEx && dbUpdateEx.InnerException is SqlException sqlEx &&
                    sqlEx.Number == 2627)
                {
                    // Registro de error de duplicación en Fac_98_Log
                    var logErrorDuplicacion = new Fac_98_Log
                    {
                        FechaHora = DateTime.Now,
                        Modulo = "Fac_04_Produccion",
                        Evento = "Error de Duplicación",
                        Detalle = $"Error de duplicación al ingresar Producción: {ex.Message}",
                        Usuario_ID = usuarioSigo
                    };
                    _context.Fac_98_Log.Add(logErrorDuplicacion);
                    await _context.SaveChangesAsync();

                    return StatusCode(409,
                        new
                        {
                            codigoEstado = 0,
                            mensaje = "Error de duplicación: El registro ya existe.",
                            detalle = sqlEx.StackTrace,
                            innerException = innerExceptionMessage
                        });
                }

                return StatusCode(500,
                    new
                    {
                        codigoEstado = 0,
                        mensaje = ex.Message,
                        detalle = ex.StackTrace,
                        innerException = innerExceptionMessage
                    });
            }
        }

        /// <summary>
        /// Modifica Producción de Parte de Pesca diario (Central).
        /// </summary>
        /// <param name="request">Los detalles de la producción a modificar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("modifica-produccion-pp")]
        public async Task<IActionResult> ModificarProduccion([FromBody] ProduccionRequest request, [FromQuery] string usuarioSigo)
        {
            // Obtener la fecha y hora del sistema
            var fechaActual = DateTime.Now.Date;
            var horaActual = DateTime.Now.TimeOfDay;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var marea = await _context.Fac_01_Mareas.FirstOrDefaultAsync(m => m.Marea_ID == request.MareaId);
                if (marea.EstadoOperativo != "en Curso")
                {
                    return BadRequest(new { codigoEstado = 0, mensaje = "Marea especificada no está en curso." });
                }

                // Validar existencia de Producto_ID y NroLance en cada línea
                foreach (var linea in request.Lineas)
                {
                    if (!await _context.Dim_09_Productos.AnyAsync(p => p.Producto_ID == linea.ProductoId))
                    {
                        return BadRequest(new { codigoEstado = 0, mensaje = $"Producto_ID especificado no existe: {linea.ProductoId}." });
                    }

                    if (linea.NroLance < 1)
                    {
                        return BadRequest(new { codigoEstado = 0, mensaje = $"NroLance debe ser mayor o igual a 1 para el Producto_ID: {linea.ProductoId}." });
                    }
                }

                // Obtener ArtePesca_ID desde Dim_03_Barcos
                var artePescaId = await _context.Dim_03_Barcos
                    .Where(b => b.Cco_ID == request.CcoId)
                    .Select(b => b.ArtePesca_ID)
                    .FirstOrDefaultAsync();

                if (artePescaId == null)
                {
                    return BadRequest(new
                    { codigoEstado = 0, mensaje = "No se encontró ArtePesca_ID asociado al Cco_ID especificado." });
                }

                int nuevaVersion = 0;

                // Procesar cada línea de producción
                foreach (var linea in request.Lineas)
                {
                    // Obtener datos de Produccion original considerando la última versión activa
                    var produccionPr = await _context.Fac_04_Produccion
                        .Where(p => p.Produccion_ID.StartsWith($"PR-{request.OperacionId}-{linea.ProductoId}-{linea.NroLance:00}") && p.Status == "A")
                        .OrderByDescending(p => p.Version)
                        .FirstOrDefaultAsync();

                    if (produccionPr == null)
                    {
                        return BadRequest(new { codigoEstado = 0, mensaje = $"No se encontró la producción original para el Producto_ID: {linea.ProductoId}." });
                    }

                    // Validar que la producción original tenga Status "A"
                    if (produccionPr.Status != "A")
                    {
                        return BadRequest(new { codigoEstado = 0, mensaje = $"La producción original no está activa para el Producto_ID: {linea.ProductoId}." });
                    }

                    // Actualizar el Status de la versión anterior a "M"
                    produccionPr.Status = "M";
                    _context.Fac_04_Produccion.Update(produccionPr);

                    var precioUsd = await _context.Dim_10_Precios
                        .Where(p => p.Producto_ID == linea.ProductoId && p.ArtePesca_ID == artePescaId && p.Estado == "A")
                        .Select(p => p.PrecioUSD)
                        .FirstOrDefaultAsync();

                    if (precioUsd == null)
                    {
                        return BadRequest(new
                        {
                            codigoEstado = 0,
                            mensaje = $"No se encontró un precio activo para el Producto_ID: {linea.ProductoId} y ArtePesca_ID especificados."
                        });
                    }

                    // Obtener el Factor desde Dim_09_Productos
                    var factor = await _context.Dim_09_Productos
                        .Where(p => p.Producto_ID == linea.ProductoId)
                        .Select(p => p.Factor)
                        .FirstOrDefaultAsync();

                    if (factor == null)
                    {
                        return BadRequest(new
                        {
                            codigoEstado = 0,
                            mensaje = $"No se encontró un Factor para el Producto_ID: {linea.ProductoId} especificado."
                        });
                    }

                    // Calcular Biomasa
                    var biomasa = linea.Kilos * factor;

                    // Generar Produccion_ID base (sin versión)
                    var produccionIdBase = $"PR-{request.OperacionId}-{linea.ProductoId}-{linea.NroLance:00}";

                    // Obtener nueva versión
                    nuevaVersion = await _context.Fac_04_Produccion
                        .Where(p => p.Produccion_ID.StartsWith(produccionIdBase))
                        .MaxAsync(p => (int?)p.Version) ?? 0;
                    nuevaVersion++;

                    // Generar Produccion_ID con versión
                    var produccionId = $"{produccionIdBase}-V{nuevaVersion:00}";

                    // Crear nueva producción 
                    var nuevaProduccion = new Fac_04_Produccion
                    {
                        Produccion_ID = produccionId,
                        Version = nuevaVersion,
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
                    };

                    // Inserción
                    _context.Fac_04_Produccion.Add(nuevaProduccion);
                }

                await _context.SaveChangesAsync();

                // Registro en Fac_99_Sync
                var log = new Fac_99_Sync
                {
                    Orden = $"DATA-PR-{request.Fecha:yyyyMMdd}-{request.CcoId}-V{nuevaVersion:00}",
                    Fecha = fechaActual,
                    Hora = horaActual,
                    TipoEvento = "Modifica Parte Pesca",
                    Origen = "Central",
                    Detalle = $"Producción modificada: Operacion_ID = {request.OperacionId}",
                    Usuario_ID = usuarioSigo
                };

                _context.Fac_99_Sync.Add(log);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    codigoEstado = 1,
                    mensaje = "Modificación exitosa. Nueva versión generada."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // Manejo de errores generales
                var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _logger.LogError(ex, "Error general.");

                // Log adicional para verificar el registro de errores
                _logger.LogInformation("Registrando error en Fac_98_Log");

                // Registro de error en Fac_98_Log
                var logError = new Fac_98_Log
                {
                    FechaHora = DateTime.Now,
                    Modulo = "Fac_04_Produccion",
                    Evento = "Error",
                    Detalle = $"Error al modificar Producción: {ex.Message}",
                    Usuario_ID = usuarioSigo
                };
                _context.Fac_98_Log.Add(logError);
                await _context.SaveChangesAsync();

                // Manejo específico de errores de valores nulos
                if (ex is SqlNullValueException)
                {
                    return StatusCode(500,
                        new
                        {
                            codigoEstado = 0,
                            mensaje = "Error de valor nulo en la base de datos.",
                            detalle = ex.StackTrace
                        });
                }

                return StatusCode(500,
                    new
                    {
                        codigoEstado = 0,
                        mensaje = ex.Message,
                        detalle = ex.StackTrace,
                        innerException = innerExceptionMessage
                    });
            }
        }

        /// <summary>
        /// Ingresa Certificación de Marea (Central).
        /// </summary>
        /// <param name="request">Los detalles de la certificación a registrar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("certificacion-marea")]
        public async Task<IActionResult> IngresarCertificacion([FromBody] CertificacionRequest request, [FromQuery] string usuarioSigo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validar que la marea está en estado "en Descarga"
                var marea = await _context.Fac_01_Mareas.FirstOrDefaultAsync(m =>
                    m.Marea_ID == request.MareaId && m.EstadoOperativo == "en Descarga");
                if (marea == null)
                {
                    return BadRequest(new
                        { codigoEstado = 0, mensaje = "La marea especificada no está en estado 'en Descarga'." });
                }

                // Validar que hay registros de producción pendientes de certificación y en Status "A"
                if (!await _context.Fac_04_Produccion.AnyAsync(p =>
                        p.Marea_ID == request.MareaId && p.Certificacion == "N" && p.Status == "A"))
                {
                    return BadRequest(new
                    {
                        codigoEstado = 0,
                        mensaje =
                            "No se encontraron registros de producción pendientes de certificación para esta marea."
                    });
                }

                // Calcular el total de kilos y cajas informados por los barcos
                var totalKilos = await _context.Fac_04_Produccion
                    .Where(p => p.Marea_ID == request.MareaId && p.Certificacion == "N" && p.Status == "A")
                    .SumAsync(p => p.Kilos);

                var totalCajas = await _context.Fac_04_Produccion
                    .Where(p => p.Marea_ID == request.MareaId && p.Certificacion == "N" && p.Status == "A")
                    .SumAsync(p => p.Cajas);

                // Actualizar los valores de PorcentajeProduccion y KgsCertificados en registros temporales
                var produccionResumen = await _context.Fac_04_Produccion
                    .Where(p => p.Marea_ID == request.MareaId && p.Certificacion == "N" && p.Status == "A")
                    .Select(p => new
                    {
                        p.Produccion_ID,
                        p.Producto_ID,
                        p.Kilos,
                        p.Cajas,
                        PorcentajeProduccion = p.Kilos / totalKilos,
                        KgsCertificados = (p.Kilos / totalKilos) * request.TotalCertificado
                    })
                    .ToListAsync();

                // Mostrar resumen al usuario
                var resumen = produccionResumen.Select(p => new
                {
                    p.Produccion_ID,
                    p.Producto_ID,
                    p.Kilos,
                    p.Cajas,
                    p.PorcentajeProduccion,
                    p.KgsCertificados,
                    TotalCertificado = request.TotalCertificado,
                    AjusteDescarga = request.TotalCertificado - totalKilos
                });

                // Confirmar transacción
                await transaction.CommitAsync();

                return Ok(new
                {
                    codigoEstado = 1,
                    mensaje = "Resumen generado correctamente. Revise y confirme.",
                    resumen
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // Manejo de errores generales
                var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _logger.LogError(ex, "Error general.");

                // Log adicional para verificar el registro de errores
                _logger.LogInformation("Registrando error en Fac_98_Log");

                // Registro de error en Fac_98_Log
                var logError = new Fac_98_Log
                {
                    FechaHora = DateTime.Now,
                    Modulo = "Fac_04_Produccion",
                    Evento = "Error",
                    Detalle = $"Error al ingresar Certificación: {ex.Message}",
                    Usuario_ID = usuarioSigo
                };
                _context.Fac_98_Log.Add(logError);
                await _context.SaveChangesAsync();

                // Manejo específico de errores de valores nulos
                if (ex is SqlNullValueException)
                {
                    return StatusCode(500,
                        new
                        {
                            codigoEstado = 0,
                            mensaje = "Error de valor nulo en la base de datos.",
                            detalle = ex.StackTrace
                        });
                }

                return StatusCode(500,
                    new
                    {
                        codigoEstado = 0,
                        mensaje = ex.Message,
                        detalle = ex.StackTrace,
                        innerException = innerExceptionMessage
                    });
            }
        }

        /// <summary>
        /// Registra Descarga de Producción, deja en CERO la Producción de la marea (Central).
        /// </summary>
        /// <param name="request">Los detalles de la Descarga a registrar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("ingresa-descarga")]
        public async Task<IActionResult> RegistrarDescarga([FromBody] DescargaRequest request, [FromQuery] string usuarioSigo)
        {
            // Obtener la fecha y hora del sistema
            var fechaActual = DateTime.Now.Date;
            var horaActual = DateTime.Now.TimeOfDay;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validar que la marea está en estado "en Descarga"
                var marea = await _context.Fac_01_Mareas.FirstOrDefaultAsync(m =>
                    m.Marea_ID == request.MareaId && m.EstadoOperativo == "en Descarga");
                if (marea == null)
                {
                    return BadRequest(new
                        { codigoEstado = 0, mensaje = "La marea especificada no está en estado 'en Descarga'." });
                }

                // Obtener ArtePesca_ID
                var artePescaId = await _context.Dim_03_Barcos
                    .Where(b => b.Cco_ID == marea.Cco_ID)
                    .Select(b => b.ArtePesca_ID)
                    .FirstOrDefaultAsync();

                if (artePescaId == null)
                {
                    return BadRequest(new
                        { codigoEstado = 0, mensaje = "No se encontró ArtePesca_ID asociado al barco." });
                }

                // Calcular total de kilos Produccion de la marea
                var totalKilosPR = await _context.Fac_04_Produccion
                    .Where(p => p.Produccion_ID.StartsWith("PR-") && p.Marea_ID == request.MareaId && p.Status == "A")
                    .SumAsync(p => p.Kilos);

                if (totalKilosPR == 0)
                {
                    return BadRequest(new
                    {
                        codigoEstado = 0,
                        mensaje = "No se encontraron datos de producción para la marea especificada."
                    });
                }

                // Calcular total de cajas Producción de la marea.
                var totalCajasPR = await _context.Fac_04_Produccion
                    .Where(p => (p.Produccion_ID.StartsWith("PR-")) && p.Marea_ID == request.MareaId && p.Status == "A")
                    .SumAsync(p => p.Cajas);

                // Insertar registros de Descarga agrupados por Producto_ID
                var producciones = await _context.Fac_04_Produccion
                    .Where(p => p.Marea_ID == request.MareaId && p.Certificacion == "N" && p.Status == "A")
                    .GroupBy(p => p.Producto_ID)
                    .Select(g => new Fac_04_Produccion
                    {
                        Produccion_ID = $"DC-OP-{marea.Cco_ID}-{request.FechaCertificacion:yyyymmdd}-V99-{g.Key}-99-V99",
                        Version = 99,
                        Operacion_ID = $"OP-{marea.Cco_ID}-{request.FechaCertificacion:yyyymmdd}-V99",
                        Fecha = fechaActual,
                        Cco_ID = marea.Cco_ID,
                        Marea_ID = request.MareaId,
                        Producto_ID = g.Key,
                        Kilos = g.Sum(p => p.Kilos) * -1,
                        Biomasa = g.Sum(p => p.Biomasa) * -1,
                        PrecioUSD = g.Sum(p => p.Kilos) > 0 ? g.Sum(p => p.TotalUSD) / g.Sum(p => p.Kilos) : 0,
                        TotalUSD = g.Sum(p => p.TotalUSD) * -1,
                        PrecioBio = g.Sum(p => p.Biomasa) > 0 ? g.Sum(p => p.TotalUSD) / g.Sum(p => p.Biomasa) : 0,
                        PorcentajeProduccion = totalKilosPR > 0 ? g.Sum(p => p.Kilos) / totalKilosPR : 0,
                        KgsCertificados = g.Sum(p => p.KgsCertificados) * -1,
                        Sincronizado = "N",
                        NroLance = 99,
                        Certificacion = "N",
                        FechaCertificacion = request.FechaCertificacion,
                        NroCertificacion = request.NroCertificacion,
                        Cajas = g.Sum(p => p.Cajas) * -1,
                        PesoMedio= g.Sum(p => p.Kilos) / g.Sum(p => p.Cajas),
                        Status = "A"
                    })
                    .ToListAsync();

                _context.Fac_04_Produccion.AddRange(producciones);
                await _context.SaveChangesAsync();

                // Registro en Fac_99_Sync
                var log = new Fac_99_Sync
                {
                    Orden = $"DATA-DC-{request.FechaCertificacion:yyyyMMdd}-{marea.Cco_ID}-V99",
                    Fecha = fechaActual,
                    Hora = horaActual,
                    TipoEvento = "Descarga",
                    Origen = "Central",
                    Detalle =
                        $"Descarga Marea: Marea_ID = {request.MareaId}, Fecha = {request.FechaCertificacion:yyyy-MM-dd}",
                    Usuario_ID = usuarioSigo
                };

                _context.Fac_99_Sync.Add(log);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    codigoEstado = 1,
                    mensaje = "Movimiento Descarga registrados correctamente."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // Manejo de errores generales
                var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _logger.LogError(ex, "Error general.");

                // Log adicional para verificar el registro de errores
                _logger.LogInformation("Registrando error en Fac_98_Log");

                // Registro de error en Fac_98_Log
                var logError = new Fac_98_Log
                {
                    FechaHora = DateTime.Now,
                    Modulo = "Fac_04_Produccion",
                    Evento = "Error",
                    Detalle = $"Error al registrar Descarga: {ex.Message}",
                    Usuario_ID = usuarioSigo
                };
                _context.Fac_98_Log.Add(logError);
                await _context.SaveChangesAsync();

                // Manejo específico de errores de valores nulos
                if (ex is SqlNullValueException)
                {
                    return StatusCode(500,
                        new
                        {
                            codigoEstado = 0,
                            mensaje = "Error de valor nulo en la base de datos.",
                            detalle = ex.StackTrace
                        });
                }

                return StatusCode(500,
                    new
                    {
                        codigoEstado = 0,
                        mensaje = ex.Message,
                        detalle = ex.StackTrace,
                        innerException = innerExceptionMessage
                    });
            }
        }

        /// <summary>
        /// Registra Certificación de Producción, registra Producción final de la marea (Central).
        /// </summary>
        /// <param name="request">Los detalles de la Certificación a registrar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("ingresa-certificacion")]
        public async Task<IActionResult> RegistrarCertificacion([FromBody] CertificaRequest request, [FromQuery] string usuarioSigo)
        {
            // Obtener la fecha y hora del sistema
            var fechaActual = DateTime.Now.Date;
            var horaActual = DateTime.Now.TimeOfDay;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validar que la marea está en estado "en Descarga"
                var marea = await _context.Fac_01_Mareas.FirstOrDefaultAsync(m =>
                    m.Marea_ID == request.MareaId && m.EstadoOperativo == "en Descarga");
                if (marea == null)
                {
                    return BadRequest(new
                        { codigoEstado = 0, mensaje = "La marea especificada no está en estado 'en Descarga'." });
                }

                // Obtener ArtePesca_ID relacionado al barco
                var artePescaId = await _context.Dim_03_Barcos
                    .Where(b => b.Cco_ID == marea.Cco_ID)
                    .Select(b => b.ArtePesca_ID)
                    .FirstOrDefaultAsync();

                if (artePescaId == null)
                {
                    return BadRequest(new
                        { codigoEstado = 0, mensaje = "No se encontró ArtePesca_ID asociado al barco." });
                }

                // Obtener Factor y PrecioUSD para el Producto_ID
                var producto = await _context.Dim_09_Productos
                    .Where(p => p.Producto_ID == request.ProductoId)
                    .Select(p => new
                    {
                        p.Factor,
                        PrecioUSD = _context.Dim_10_Precios
                            .Where(pr =>
                                pr.Producto_ID == p.Producto_ID && pr.ArtePesca_ID == artePescaId && pr.Estado == "A")
                            .Select(pr => pr.PrecioUSD)
                            .FirstOrDefault()
                    })
                    .FirstOrDefaultAsync();

                if (producto == null || producto.Factor == null || producto.PrecioUSD == null)
                {
                    return BadRequest(new
                    {
                        codigoEstado = 0, mensaje = "No se encontró precio o factor para el Producto_ID especificado."
                    });
                }

                // Validar que Produccion_ID no exista
                var produccionId = $"CF-{marea.Cco_ID}-{request.FechaCertificacion}-V99-{request.ProductoId}-99-V99";
                if (await _context.Fac_04_Produccion.AnyAsync(p => p.Produccion_ID == produccionId))
                {
                    return BadRequest(new
                        { codigoEstado = 0, mensaje = "El ID de producción ya existe. Verifique los datos." });
                }

                // Calcular Biomasa
                var biomasa = request.Kilos * producto.Factor;

                // Insertar registro de Certificación de Producción
                var nuevaProduccion = new Fac_04_Produccion
                {
                    Produccion_ID = produccionId,
                    Version = 99,
                    Operacion_ID = $"OP-{marea.Cco_ID}-{request.FechaCertificacion}-V99",
                    Fecha = request.FechaCertificacion,
                    Cco_ID = marea.Cco_ID,
                    Marea_ID = request.MareaId,
                    Producto_ID = request.ProductoId,
                    Kilos = request.Kilos,
                    Biomasa = biomasa,
                    PrecioUSD = producto.PrecioUSD,
                    TotalUSD = request.Kilos * producto.PrecioUSD,
                    PrecioBio = biomasa > 0 ? (request.Kilos * producto.PrecioUSD) / biomasa : 0,
                    PorcentajeProduccion = 1, // Proporción predeterminada para CPR
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

                _context.Fac_04_Produccion.Add(nuevaProduccion);
                await _context.SaveChangesAsync();

                // Registro en Fac_99_Sync
                var log = new Fac_99_Sync
                {
                    Orden = $"DATA-CF-{request.FechaCertificacion:yyyyMMdd}-{marea.Cco_ID}-V99",
                    Fecha = fechaActual,
                    Hora = horaActual,
                    TipoEvento = "Certificación",
                    Origen = "Central",
                    Detalle =
                        $"Certificación Marea: Marea_ID = {request.MareaId}, Fecha = {request.FechaCertificacion:yyyy-MM-dd}",
                    Usuario_ID = usuarioSigo
                };

                _context.Fac_99_Sync.Add(log);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    codigoEstado = 1,
                    mensaje = "Registro de Certificación realizado correctamente."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // Manejo de errores generales
                var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _logger.LogError(ex, "Error general.");

                // Log adicional para verificar el registro de errores
                _logger.LogInformation("Registrando error en Fac_98_Log");

                // Registro de error en Fac_98_Log
                var logError = new Fac_98_Log
                {
                    FechaHora = DateTime.Now,
                    Modulo = "Fac_04_Produccion",
                    Evento = "Error",
                    Detalle = $"Error al registrar Certificación: {ex.Message}",
                    Usuario_ID = usuarioSigo
                };
                _context.Fac_98_Log.Add(logError);
                await _context.SaveChangesAsync();

                // Manejo específico de errores de valores nulos
                if (ex is SqlNullValueException)
                {
                    return StatusCode(500,
                        new
                        {
                            codigoEstado = 0,
                            mensaje = "Error de valor nulo en la base de datos.",
                            detalle = ex.StackTrace
                        });
                }

                return StatusCode(500,
                    new
                    {
                        codigoEstado = 0,
                        mensaje = ex.Message,
                        detalle = ex.StackTrace,
                        innerException = innerExceptionMessage
                    });
            }
        }

        /// <summary>
        /// Cambia el estado de Marea al finalizar Descarga/Certificación (Central).
        /// </summary>
        /// <param name="request">Los detalles del cambio de estado.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("cambio-estado-mareas")]
        public async Task<IActionResult> CambiarEstadoMareas([FromQuery] string mareaId, [FromQuery] string usuarioSigo)
        {
            // Obtener la fecha y hora del sistema
            var fechaActual = DateTime.Now.Date;
            var horaActual = DateTime.Now.TimeOfDay;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validar que la marea está en estado "en Descarga"
                var marea = await _context.Fac_01_Mareas.FirstOrDefaultAsync(m => m.Marea_ID == mareaId && m.EstadoOperativo == "en Descarga");
                if (marea == null)
                {
                    return BadRequest(new { codigoEstado = 0, mensaje = "La marea especificada no está en estado 'en Descarga'." });
                }

                // Validar que existan registros en Fac_04_Produccion para la Marea_ID
                if (!await _context.Fac_04_Produccion.AnyAsync(p => p.Marea_ID == mareaId))
                {
                    return BadRequest(new { codigoEstado = 0, mensaje = "No se encontraron registros de producción para la marea especificada." });
                }

                // Actualizar Certificacion en Fac_04_Produccion
                var producciones = await _context.Fac_04_Produccion.Where(p => p.Marea_ID == mareaId).ToListAsync();
                foreach (var produccion in producciones)
                {
                    produccion.Certificacion = "S";
                }
                _context.Fac_04_Produccion.UpdateRange(producciones);
                await _context.SaveChangesAsync();

                // Actualizar EstadoOperativo en Fac_01_Mareas
                marea.EstadoOperativo = "Certificada";
                marea.Estado = "I";
                _context.Fac_01_Mareas.Update(marea);
                await _context.SaveChangesAsync();

                // Registrar el cambio en Fac_98_Log
                var log = new Fac_98_Log
                {
                    FechaHora = DateTime.Now,
                    Modulo = "Fac_04_Produccion",
                    Evento = "Cambio de Estado",
                    Detalle = $"Cambio de estado de Marea: Marea_ID = {mareaId}, Nuevo Estado = {marea.EstadoOperativo}",
                    Usuario_ID = usuarioSigo
                };
                _context.Fac_98_Log.Add(log);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { codigoEstado = 1, mensaje = "Cambio de estado realizado correctamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // Manejo de errores generales
                var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                _logger.LogError(ex, "Error general.");

                // Log adicional para verificar el registro de errores
                _logger.LogInformation("Registrando error en Fac_98_Log");

                // Registro de error en Fac_98_Log
                var logError = new Fac_98_Log
                {
                    FechaHora = DateTime.Now,
                    Modulo = "Fac_04_Produccion",
                    Evento = "Error",
                    Detalle = $"Error al cambiar estado de marea: {ex.Message}",
                    Usuario_ID = usuarioSigo
                };
                _context.Fac_98_Log.Add(logError);
                await _context.SaveChangesAsync();

                // Manejo específico de errores de valores nulos
                if (ex is SqlNullValueException)
                {
                    return StatusCode(500, new { codigoEstado = 0, mensaje = "Error de valor nulo en la base de datos.", detalle = ex.StackTrace });
                }

                return StatusCode(500, new { codigoEstado = 0, mensaje = ex.Message, detalle = ex.StackTrace, innerException = innerExceptionMessage });
            }
        }
    }
}