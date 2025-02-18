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

        [HttpPost("ingresa-operacion-pp")]
        public async Task<IActionResult> RegistrarOperacion(
            [FromBody] OperacionRequest request,
            [FromQuery] string usuarioSigo)
        {
            using var transaction = await _fac03Repository.BeginTransactionAsync();
            try
            {
                var operacionId = await _fac03Repository.RegistrarOperacionAsync(request, usuarioSigo);
                await transaction.CommitAsync();

                return Ok(new
                {
                    codigoEstado = 1,
                    mensaje = "Inserción exitosa.",
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
                    mensaje = "Error interno del servidor."
                });
            }
        }

        [HttpPost("modifica-operacion-pp")]
        public async Task<IActionResult> ModificarOperacion(
            [FromBody] OperacionRequest request,
            [FromQuery] string usuarioSigo)
        {
            using var transaction = await _fac03Repository.BeginTransactionAsync();
            try
            {
                var operacionId = await _fac03Repository.ModificarOperacionAsync(request, usuarioSigo);
                await transaction.CommitAsync();

                return Ok(new
                {
                    codigoEstado = 1,
                    mensaje = "Modificación exitosa.",
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
                    mensaje = "Error interno del servidor."
                });
            }
        }
    }
}