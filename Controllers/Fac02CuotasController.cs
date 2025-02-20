using BackEnd.Constants;
using Microsoft.AspNetCore.Mvc;
using BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using BackEnd.Filters;

namespace BackEnd.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ValidateUserFilter))]
    public class Fac02CuotasController : ControllerBase
    {
        private readonly IFac02Repository _fac02Repository;
        private readonly ILogger<Fac02CuotasController> _logger;

        public Fac02CuotasController(IFac02Repository fac02Repository, ILogger<Fac02CuotasController> logger)
        {
            _fac02Repository = fac02Repository;
            _logger = logger;
        }

        /// <summary>
        /// Agrega nuevas cuotas.
        /// </summary>
        /// <param name="request">Los detalles de la cuota a agregar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        /// /// <response code="200">Cuota registrada exitosamente.</response>
        /// <response code="400">Si los datos de entrada son inválidos.</response>
        /// <response code="500">Si ocurre un error interno en el servidor.</response>
        [HttpPost("aumenta")]
        public async Task<IActionResult> AumentoCuotas([FromBody] AumentaCuotasRequest request, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de AumentoCuotas para Cuota_ID: {Cuota_ID}, Especie_ID: {Especie_ID}", request.Cuota_ID, request.Especie_ID);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Datos inválidos para Cuota_ID: {Cuota_ID}, Especie_ID: {Especie_ID}, Zona_ID: {Zona_ID}", request.Cuota_ID, request.Especie_ID, request.Zona_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            return await ManejarOperacionAsync(
                () => _fac02Repository.AgregarCuotaAsync(request, usuarioSigo),
                Messages.RegistroInsertado);
        }

        /// <summary>
        /// Agrega traspasos de cuotas a terceros.
        /// </summary>
        /// <param name="request">Los detalles del traspaso de cuota.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        /// <response code="200">Cuota Traspasada exitosamente.</response>
        /// <response code="400">Si los datos de entrada son inválidos.</response>
        /// <response code="500">Si ocurre un error interno en el servidor.</response>
        [HttpPost("rebaja")]
        public async Task<IActionResult> TraspasoCuotas([FromBody] RebajaCuotasRequest request, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de TraspasoCuotas para Cuota_ID: {Cuota_ID}, Especie_ID: {Especie_ID}", request.Cuota_ID, request.Especie_ID);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Datos inválidos para Cuota_ID: {Cuota_ID}, Especie_ID: {Especie_ID}, Zona_ID: {Zona_ID}", request.Cuota_ID, request.Especie_ID, request.Zona_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            return await ManejarOperacionAsync(
                () => _fac02Repository.TraspasarCuotaAsync(request, usuarioSigo),
                Messages.RegistroInsertado);
        }

        /// <summary>
        /// Traspasa Cuotas entre Zonas de Pesca.
        /// </summary>
        /// <param name="request">Los detalles del cambio de zona.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        /// <response code="200">Cuota Cambiada de Zona exitosamente.</response>
        /// <response code="400">Si los datos de entrada son inválidos.</response>
        /// <response code="500">Si ocurre un error interno en el servidor.</response>
        [HttpPost("cambio-zona")]
        public async Task<IActionResult> CambioZona([FromBody] CambioZonaRequest request, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de CambioZona para Cuota_ID: {Cuota_ID}, Especie_ID: {Especie_ID}", request.Cuota_ID, request.Especie_ID);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Datos inválidos para Cuota_ID: {Cuota_ID}, Especie_ID: {Especie_ID}, ZonaOrigen_ID: {ZonaOrigen_ID}, ZonaDestino_ID: {ZonaDestino_ID}", request.Cuota_ID, request.Especie_ID, request.ZonaOrigen_ID, request.ZonaDestino_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            return await ManejarOperacionAsync(
                () => _fac02Repository.CambiarZonaCuotaAsync(request, usuarioSigo),
                Messages.RegistroInsertado);
        }

        //---------------------------------------------------------------------
        // Métodos Privados
        //---------------------------------------------------------------------
        private async Task<IActionResult> ManejarOperacionAsync(
            Func<Task<string>> operacion,
            string mensajeExito)
        {
            try
            {
                var resultado = await operacion();
                _logger.LogInformation("Operación completada exitosamente. Resultado: {Resultado}", resultado);

                return Ok(new
                {
                    codigoEstado = 1,
                    mensaje = mensajeExito,
                    resultado
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación: {Message}", ex.Message);
                return BadRequest(new { codigoEstado = 0, mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico: {Message}", ex.Message);
                return StatusCode(500, new
                {
                    codigoEstado = 0,
                    mensaje = Messages.ErrorInternoServidor
                });
            }
        }
    }
}