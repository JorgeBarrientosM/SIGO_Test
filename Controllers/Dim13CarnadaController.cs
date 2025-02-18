using Microsoft.AspNetCore.Mvc;
using BackEnd.Interfaces;
using BackEnd.Models;
using BackEnd.Data;
using Microsoft.AspNetCore.Authorization;
using BackEnd.Constants;
using BackEnd.Filters;

namespace BackEnd.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ValidateUserFilter))]
    public class Dim13CarnadaController : ControllerBase
    {
        private readonly IDimGenericRepository<Dim_13_Carnada> _dimGenericRepository;
        private readonly ILogger<Dim13CarnadaController> _logger;

        public Dim13CarnadaController(IDimGenericRepository<Dim_13_Carnada> dimGenericRepository, ILogger<Dim13CarnadaController> logger)
        {
            _dimGenericRepository = dimGenericRepository;
            _logger = logger;
        }

        /// <summary>
        /// Agrega nueva Carnada.
        /// </summary>
        /// <param name="dim13Carnada">Los detalles de la Carnada a agregar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("nuevo")]
        public async Task<IActionResult> PostDim13Carnada([FromBody] Dim_13_Carnada dim13Carnada, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PostDim13Carnada para Carnada_ID: {Carnada_ID}", dim13Carnada.Carnada_ID);

            if (string.IsNullOrEmpty(dim13Carnada.Carnada_ID))
            {
                _logger.LogWarning("Datos inválidos para Carnada_ID: {Carnada_ID}", dim13Carnada.Carnada_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            if (await _dimGenericRepository.ExistsAsync(dim13Carnada.Carnada_ID))
            {
                _logger.LogWarning("Registro duplicado para Carnada_ID: {Carnada_ID}", dim13Carnada.Carnada_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.RegistroDuplicado });
            }

            _logger.LogInformation("Agregando nueva Carnada para Carnada_ID: {Carnada_ID}", dim13Carnada.Carnada_ID);
            await _dimGenericRepository.AddAsync(dim13Carnada, usuarioSigo);
            _logger.LogInformation("Carnada agregada exitosamente para Carnada_ID: {Carnada_ID}", dim13Carnada.Carnada_ID);

            _logger.LogInformation("Fin de PostDim13Carnada para Carnada_ID: {Carnada_ID}", dim13Carnada.Carnada_ID);
            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroInsertado, carnadaId = dim13Carnada.Carnada_ID });
        }

        /// <summary>
        /// Modifica Carnada existente.
        /// </summary>
        /// <param name="id">El ID de la Carnada a modificar.</param>
        /// <param name="updateRequest">Los detalles de la actualización.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPut("{id}/modifica")]
        public async Task<IActionResult> PutDim13Carnada(string id, [FromBody] Dim13CarnadaUpdateRequest updateRequest, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PutDim13Carnada para Carnada_ID: {Carnada_ID}", id);

            var existingCarnada = await _dimGenericRepository.GetByIdAsync(id);
            if (existingCarnada == null)
            {
                _logger.LogWarning("Registro no encontrado para Carnada_ID: {Carnada_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            existingCarnada.DescripcionCarnada = updateRequest.DescripcionCarnada ?? existingCarnada.DescripcionCarnada;
            existingCarnada.CalibreCarnada = updateRequest.CalibreCarnada ?? existingCarnada.CalibreCarnada;
            existingCarnada.PiezasKg = updateRequest.PiezasKg != -1 ? updateRequest.PiezasKg : existingCarnada.PiezasKg;

            _logger.LogInformation("Modificando Carnada para Carnada_ID: {Carnada_ID}", id);
            await _dimGenericRepository.UpdateAsync(existingCarnada, usuarioSigo);
            _logger.LogInformation("Carnada modificada exitosamente para Carnada_ID: {Carnada_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroActualizado, carnadaId = id });
        }

        /// <summary>
        /// Cambia Estado de Carnada existente (Activo/Inactivo).
        /// </summary>
        /// <param name="id">El ID de la Carnada a modificar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpDelete("{id}/cambia-estado")]
        public async Task<IActionResult> DeleteDim13Carnada(string id, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de DeleteDim13Carnada para Carnada_ID: {Carnada_ID}", id);

            var existingCarnada = await _dimGenericRepository.GetByIdAsync(id);
            if (existingCarnada == null)
            {
                _logger.LogWarning("Registro no encontrado para Carnada_ID: {Carnada_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            _logger.LogInformation("Cambiando estado de Carnada para Carnada_ID: {Carnada_ID}", id);
            await _dimGenericRepository.DeleteAsync(id, usuarioSigo);
            _logger.LogInformation("Estado de Carnada cambiado exitosamente para Carnada_ID: {Carnada_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.CambioEstado, carnadaId = id, nuevoEstado = existingCarnada.Estado });
        }
    }
}