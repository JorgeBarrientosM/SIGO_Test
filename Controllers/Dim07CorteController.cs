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
    public class Dim07CorteController : ControllerBase
    {
        private readonly IDimGenericRepository<Dim_07_Corte> _dimGenericRepository;
        private readonly ILogger<Dim07CorteController> _logger;

        public Dim07CorteController(IDimGenericRepository<Dim_07_Corte> dimGenericRepository, ILogger<Dim07CorteController> logger)
        {
            _dimGenericRepository = dimGenericRepository;
            _logger = logger;
        }

        /// <summary>
        /// Agrega nuevo Corte.
        /// </summary>
        /// <param name="dim07Corte">Los detalles del Corte a agregar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("nuevo")]
        public async Task<IActionResult> PostDim07Corte([FromBody] Dim_07_Corte dim07Corte, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PostDim07Corte para Corte_ID: {Corte_ID}", dim07Corte.Corte_ID);

            if (string.IsNullOrEmpty(dim07Corte.Corte_ID))
            {
                _logger.LogWarning("Datos inválidos para Corte_ID: {Corte_ID}", dim07Corte.Corte_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            if (await _dimGenericRepository.ExistsAsync(dim07Corte.Corte_ID))
            {
                _logger.LogWarning("Registro duplicado para Corte_ID: {Corte_ID}", dim07Corte.Corte_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.RegistroDuplicado });
            }

            _logger.LogInformation("Agregando nuevo Corte para Corte_ID: {Corte_ID}", dim07Corte.Corte_ID);
            await _dimGenericRepository.AddAsync(dim07Corte, usuarioSigo);
            _logger.LogInformation("Corte agregado exitosamente para Corte_ID: {Corte_ID}", dim07Corte.Corte_ID);

            _logger.LogInformation("Fin de PostDim07Corte para Corte_ID: {Corte_ID}", dim07Corte.Corte_ID);
            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroInsertado, corteId = dim07Corte.Corte_ID });
        }

        /// <summary>
        /// Modifica Corte existente.
        /// </summary>
        /// <param name="id">El ID del Corte a modificar.</param>
        /// <param name="updateRequest">Los detalles de la actualización.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPut("{id}/modifica")]
        public async Task<IActionResult> PutDim07Corte(string id, [FromBody] Dim07CorteUpdateRequest updateRequest, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PutDim07Corte para Corte_ID: {Corte_ID}", id);

            var existingCorte = await _dimGenericRepository.GetByIdAsync(id);
            if (existingCorte == null)
            {
                _logger.LogWarning("Registro no encontrado para Corte_ID: {Corte_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            existingCorte.DescripcionCorte = updateRequest.DescripcionCorte ?? existingCorte.DescripcionCorte;
            existingCorte.TipoCorte = updateRequest.TipoCorte ?? existingCorte.TipoCorte;
            existingCorte.AgregadoCorte = updateRequest.AgregadoCorte ?? existingCorte.AgregadoCorte;

            _logger.LogInformation("Modificando Corte para Corte_ID: {Corte_ID}", id);
            await _dimGenericRepository.UpdateAsync(existingCorte, usuarioSigo);
            _logger.LogInformation("Corte modificado exitosamente para Corte_ID: {Corte_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroActualizado, corteId = id });
        }

        /// <summary>
        /// Cambia Estado de Corte (Activo/Inactivo).
        /// </summary>
        /// <param name="id">El ID del Corte a modificar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpDelete("{id}/cambia-estado")]
        public async Task<IActionResult> DeleteDim07Corte(string id, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de DeleteDim07Corte para Corte_ID: {Corte_ID}", id);

            var existingCorte = await _dimGenericRepository.GetByIdAsync(id);
            if (existingCorte == null)
            {
                _logger.LogWarning("Registro no encontrado para Corte_ID: {Corte_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            _logger.LogInformation("Cambiando estado de Corte para Corte_ID: {Corte_ID}", id);
            await _dimGenericRepository.DeleteAsync(id, usuarioSigo);
            _logger.LogInformation("Estado de Corte cambiado exitosamente para Corte_ID: {Corte_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.CambioEstado, corteId = id, nuevoEstado = existingCorte.Estado });
        }
    }
}