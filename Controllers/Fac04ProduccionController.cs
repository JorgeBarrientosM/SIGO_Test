using BackEnd.Constants;
using Microsoft.AspNetCore.Mvc;
using BackEnd.Models;
using BackEnd.Interfaces;
using Microsoft.AspNetCore.Authorization;
using BackEnd.Filters;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ValidateUserFilter))]
    public class Fac04ProduccionController : ControllerBase
    {
        private readonly IFac04Repository _fac04Repository;
        private readonly ILogger<Fac04ProduccionController> _logger;

        public Fac04ProduccionController(
            IFac04Repository fac04Repository,
            ILogger<Fac04ProduccionController> logger)
        {
            _fac04Repository = fac04Repository ?? throw new ArgumentNullException(nameof(fac04Repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Registra una nueva producción.
        /// </summary>
        /// <param name="request">Datos de la producción.</param>
        /// <param name="usuarioSigo">Usuario que realiza la operación.</param>
        /// <returns>Resultado de la operación.</returns>
        /// <response code="200">Producción registrada exitosamente.</response>
        /// <response code="400">Si los datos de entrada son inválidos.</response>
        /// <response code="500">Si ocurre un error interno en el servidor.</response>
        [HttpPost("ingresa-produccion-pp")]
        public async Task<IActionResult> RegistrarProduccion(
            [FromBody] ProduccionRequest request,
            [FromQuery] string usuarioSigo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            if (string.IsNullOrEmpty(usuarioSigo))
            {
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.UsuarioNoValido });
            }

            return await ManejarOperacionAsync(
                () => _fac04Repository.RegistrarProduccionAsync(request, usuarioSigo),
                Messages.RegistroInsertado);
        }

        /// <summary>
        /// Modifica una producción existente.
        /// </summary>
        /// <param name="request">Datos de la producción.</param>
        /// <param name="usuarioSigo">Usuario que realiza la operación.</param>
        /// <returns>Resultado de la operación.</returns>
        /// <response code="200">Producción modificada exitosamente.</response>
        /// <response code="400">Si los datos de entrada son inválidos.</response>
        /// <response code="500">Si ocurre un error interno en el servidor.</response>
        [HttpPost("modifica-produccion-pp")]
        public async Task<IActionResult> ModificarProduccion(
            [FromBody] ProduccionRequest request,
            [FromQuery] string usuarioSigo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            if (string.IsNullOrEmpty(usuarioSigo))
            {
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.UsuarioNoValido });
            }

            return await ManejarOperacionAsync(
                () => _fac04Repository.ModificarProduccionAsync(request, usuarioSigo),
                Messages.RegistroActualizado);
        }

        /// <summary>
        /// Ingresa la certificación de una marea.
        /// </summary>
        /// <param name="request">Datos de la certificación.</param>
        /// <param name="usuarioSigo">Usuario que realiza la operación.</param>
        /// <returns>Resultado de la operación.</returns>
        /// <response code="200">Certificación ingresada exitosamente.</response>
        /// <response code="400">Si los datos de entrada son inválidos.</response>
        /// <response code="500">Si ocurre un error interno en el servidor.</response>
        [HttpPost("certificacion-marea")]
        public async Task<IActionResult> IngresarCertificacion(
            [FromBody] CertificacionRequest request,
            [FromQuery] string usuarioSigo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            if (string.IsNullOrEmpty(usuarioSigo))
            {
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.UsuarioNoValido });
            }

            return await ManejarOperacionAsync(
                () => _fac04Repository.IngresarCertificacionAsync(request, usuarioSigo),
                Messages.RegistroInsertado);
        }

        /// <summary>
        /// Registra la descarga de producción.
        /// </summary>
        /// <param name="request">Datos de la descarga.</param>
        /// <param name="usuarioSigo">Usuario que realiza la operación.</param>
        /// <returns>Resultado de la operación.</returns>
        /// <response code="200">Descarga registrada exitosamente.</response>
        /// <response code="400">Si los datos de entrada son inválidos.</response>
        /// <response code="500">Si ocurre un error interno en el servidor.</response>
        [HttpPost("ingresa-descarga")]
        public async Task<IActionResult> RegistrarDescarga(
            [FromBody] DescargaRequest request,
            [FromQuery] string usuarioSigo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            if (string.IsNullOrEmpty(usuarioSigo))
            {
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.UsuarioNoValido });
            }

            return await ManejarOperacionAsync(
                () => _fac04Repository.RegistrarDescargaAsync(request, usuarioSigo),
                Messages.RegistroInsertado);
        }

        /// <summary>
        /// Registra la certificación final de producción.
        /// </summary>
        /// <param name="request">Datos de la certificación.</param>
        /// <param name="usuarioSigo">Usuario que realiza la operación.</param>
        /// <returns>Resultado de la operación.</returns>
        /// <response code="200">Certificación registrada exitosamente.</response>
        /// <response code="400">Si los datos de entrada son inválidos.</response>
        /// <response code="500">Si ocurre un error interno en el servidor.</response>
        [HttpPost("ingresa-certificacion")]
        public async Task<IActionResult> RegistrarCertificacionFinal(
            [FromBody] CertificaRequest request,
            [FromQuery] string usuarioSigo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            if (string.IsNullOrEmpty(usuarioSigo))
            {
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.UsuarioNoValido });
            }

            return await ManejarOperacionAsync(
                () => _fac04Repository.RegistrarCertificacionFinalAsync(request, usuarioSigo),
                Messages.RegistroInsertado);
        }

        /// <summary>
        /// Cambia el estado de una marea.
        /// </summary>
        /// <param name="mareaId">ID de la marea.</param>
        /// <param name="usuarioSigo">Usuario que realiza la operación.</param>
        /// <returns>Resultado de la operación.</returns>
        /// <response code="200">Estado de la marea cambiado exitosamente.</response>
        /// <response code="400">Si los datos de entrada son inválidos.</response>
        /// <response code="500">Si ocurre un error interno en el servidor.</response>
        [HttpPost("cambio-estado-mareas")]
        public async Task<IActionResult> CambiarEstadoMarea(
            [FromQuery] string mareaId,
            [FromQuery] string usuarioSigo)
        {
            if (string.IsNullOrEmpty(mareaId))
            {
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            if (string.IsNullOrEmpty(usuarioSigo))
            {
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.UsuarioNoValido });
            }

            return await ManejarOperacionAsync(
                () => _fac04Repository.CambiarEstadoMareaAsync(mareaId, usuarioSigo),
                Messages.CambioEstado);
        }

        //---------------------------------------------------------------------
        // Métodos Privados
        //---------------------------------------------------------------------
        private async Task<IActionResult> ManejarOperacionAsync(Func<Task<string>> operacion, string mensajeExito)
        {
            using var transaction = await _fac04Repository.BeginTransactionAsync();
            try
            {
                var resultado = await operacion();
                if (resultado == null)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new { codigoEstado = 0, mensaje = "Operación no devolvió un resultado válido." });
                }

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Operación completada exitosamente. Mensaje: {MensajeExito}",
                    mensajeExito
                );

                return Ok(new
                {
                    codigoEstado = 1,
                    mensaje = mensajeExito,
                    resultado
                });
            }
            catch (ArgumentException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogWarning(ex, "Error de validación: {Message}", ex.Message);
                return BadRequest(new { codigoEstado = 0, mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
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