using Microsoft.AspNetCore.Mvc;
using BackEnd.Interfaces;
using BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using BackEnd.Constants;
using BackEnd.Filters;
using BackEnd.Data;

namespace BackEnd.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ValidateUserFilter))]
    public class Dim01CcoController : ControllerBase
    {
        private readonly IDimGenericRepository<Dim_01_Cco> _dimGenericRepository;
        private readonly ILogger<Dim01CcoController> _logger;

        public Dim01CcoController(IDimGenericRepository<Dim_01_Cco> dimGenericRepository, ILogger<Dim01CcoController> logger)
        {
            _dimGenericRepository = dimGenericRepository;
            _logger = logger;
        }

        /// <summary>
        /// Agrega nuevo Centro de Costos.
        /// </summary>
        /// <param name="dim01Cco">Los detalles del CCO a agregar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("nuevo")]
        public async Task<IActionResult> PostDim01Cco([FromBody] Dim_01_Cco dim01Cco, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PostDim01Cco para Cco_ID: {Cco_ID}", dim01Cco.Cco_ID);

            if (string.IsNullOrEmpty(dim01Cco.Cco_ID))
            {
                _logger.LogWarning("Datos inválidos para Cco_ID: {Cco_ID}", dim01Cco.Cco_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            if (await _dimGenericRepository.ExistsAsync(dim01Cco.Cco_ID))
            {
                _logger.LogWarning("Registro duplicado para Cco_ID: {Cco_ID}", dim01Cco.Cco_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.RegistroDuplicado });
            }

            _logger.LogInformation("Agregando nuevo Centro de Costos para Cco_ID: {Cco_ID}", dim01Cco.Cco_ID);
            await _dimGenericRepository.AddAsync(dim01Cco, usuarioSigo);
            _logger.LogInformation("Centro de Costos agregado exitosamente para Cco_ID: {Cco_ID}", dim01Cco.Cco_ID);

            _logger.LogInformation("Fin de PostDim01Cco para Cco_ID: {Cco_ID}", dim01Cco.Cco_ID);
            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroInsertado, ccoId = dim01Cco.Cco_ID });
        }

        /// <summary>
        /// Modifica Centro de Costos existente.
        /// </summary>
        /// <param name="id">El ID del CCO a modificar.</param>
        /// <param name="updateRequest">Los detalles de la actualización.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPut("{id}/modifica")]
        public async Task<IActionResult> PutDim01Cco(string id, [FromBody] Dim01CcoUpdateRequest updateRequest, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PutDim01Cco para Cco_ID: {Cco_ID}", id);

            var existingCco = await _dimGenericRepository.GetByIdAsync(id);
            if (existingCco == null)
            {
                _logger.LogWarning("Registro no encontrado para Cco_ID: {Cco_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            existingCco.NombreCco = updateRequest.NombreCco ?? existingCco.NombreCco;
            existingCco.TipoCco = updateRequest.TipoCco ?? existingCco.TipoCco;

            _logger.LogInformation("Modificando Centro de Costos para Cco_ID: {Cco_ID}", id);
            await _dimGenericRepository.UpdateAsync(existingCco, usuarioSigo);
            _logger.LogInformation("Centro de Costos modificado exitosamente para Cco_ID: {Cco_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroActualizado, ccoId = id });
        }

        /// <summary>
        /// Cambia Estado de Centro de Costos existente (Activo/Inactivo).
        /// </summary>
        /// <param name="id">El ID del CCO a modificar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpDelete("{id}/cambia-estado")]
        public async Task<IActionResult> DeleteDim01Cco(string id, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de DeleteDim01Cco para Cco_ID: {Cco_ID}", id);

            var existingCco = await _dimGenericRepository.GetByIdAsync(id);
            if (existingCco == null)
            {
                _logger.LogWarning("Registro no encontrado para Cco_ID: {Cco_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            _logger.LogInformation("Cambiando estado de Centro de Costos para Cco_ID: {Cco_ID}", id);
            await _dimGenericRepository.DeleteAsync(id, usuarioSigo);
            _logger.LogInformation("Estado de Centro de Costos cambiado exitosamente para Cco_ID: {Cco_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.CambioEstado, ccoId = id, nuevoEstado = existingCco.Estado });
        }
    }
}