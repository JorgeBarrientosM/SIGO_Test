using BackEnd.Constants;
using Microsoft.AspNetCore.Mvc;
using BackEnd.Models;
using BackEnd.Interfaces;
using Microsoft.AspNetCore.Authorization;
using BackEnd.Filters;

namespace BackEnd.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ValidateUserFilter))]
    public class Fac03OperacionController : ControllerBase
    {
        private readonly IFac03Repository _fac03Repository;
        private readonly ILogger<Fac03OperacionController> _logger;

        public Fac03OperacionController(
            IFac03Repository fac03Repository,
            ILogger<Fac03OperacionController> logger)
        {
            _fac03Repository = fac03Repository;
            _logger = logger;
        }

        /// <summary>
        /// Registra una nueva operación.
        /// </summary>
        /// <param name="request">Datos de la operación.</param>
        /// <param name="usuarioSigo">Usuario que realiza la operación.</param>
        /// <returns>Resultado de la operación.</returns>
        /// <response code="200">Operación registrada exitosamente.</response>
        /// <response code="400">Si los datos de entrada son inválidos.</response>
        /// <response code="500">Si ocurre un error interno en el servidor.</response>
        [HttpPost("ingresa-operacion-pp")]
        public async Task<IActionResult> RegistrarOperacion(
            [FromBody] OperacionRequest request,
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
                () => _fac03Repository.RegistrarOperacionAsync(request, usuarioSigo),
                Messages.RegistroInsertado);
        }

        /// <summary>
        /// Modifica una operación existente.
        /// </summary>
        /// <param name="request">Datos de la operación.</param>
        /// <param name="usuarioSigo">Usuario que realiza la operación.</param>
        /// <returns>Resultado de la operación.</returns>
        /// <response code="200">Operación modificada exitosamente.</response>
        /// <response code="400">Si los datos de entrada son inválidos.</response>
        /// <response code="500">Si ocurre un error interno en el servidor.</response>
        [HttpPost("modifica-operacion-pp")]
        public async Task<IActionResult> ModificarOperacion(
            [FromBody] OperacionRequest request,
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
                () => _fac03Repository.ModificarOperacionAsync(request, usuarioSigo),
                Messages.RegistroActualizado);
        }

        //---------------------------------------------------------------------
        // Métodos Privados
        //---------------------------------------------------------------------
        private async Task<IActionResult> ManejarOperacionAsync(
            Func<Task<string>> operacion,
            string mensajeExito)
        {
            using var transaction = await _fac03Repository.BeginTransactionAsync();
            try
            {
                var operacionId = await operacion();
                await transaction.CommitAsync();

                // Registro de log informativo para transacciones exitosas
                _logger.LogInformation(
                    "Operación {OperacionId} completada exitosamente. Mensaje: {MensajeExito}",
                    operacionId,
                    mensajeExito
                );

                return Ok(new
                {
                    codigoEstado = 1,
                    mensaje = mensajeExito,
                    operacionId
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