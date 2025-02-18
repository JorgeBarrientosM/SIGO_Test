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
    public class Dim12ObjetivoController : ControllerBase
    {
        private readonly IDimGenericRepository<Dim_12_Objetivo> _dimGenericRepository;
        private readonly ILogger<Dim12ObjetivoController> _logger;

        public Dim12ObjetivoController(IDimGenericRepository<Dim_12_Objetivo> dimGenericRepository,  ILogger<Dim12ObjetivoController> logger)
        {
            _dimGenericRepository = dimGenericRepository;
            _logger = logger;
        }

        /// <summary>
        /// Agrega nuevo Objetivo.
        /// </summary>
        /// <param name="dim12Objetivo">Los detalles del Objetivo a agregar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("nuevo")]
        public async Task<IActionResult> PostDim12Objetivo([FromBody] Dim_12_Objetivo dim12Objetivo, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PostDim12Objetivo para Objetivo_ID: {Objetivo_ID}", dim12Objetivo.Objetivo_ID);

            if (string.IsNullOrEmpty(dim12Objetivo.Objetivo_ID))
            {
                _logger.LogWarning("Datos inválidos para Objetivo_ID: {Objetivo_ID}", dim12Objetivo.Objetivo_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            if (await _dimGenericRepository.ExistsAsync(dim12Objetivo.Objetivo_ID))
            {
                _logger.LogWarning("Registro duplicado para Objetivo_ID: {Objetivo_ID}", dim12Objetivo.Objetivo_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.RegistroDuplicado });
            }

            _logger.LogInformation("Agregando nuevo Objetivo para Objetivo_ID: {Objetivo_ID}", dim12Objetivo.Objetivo_ID);
            await _dimGenericRepository.AddAsync(dim12Objetivo, usuarioSigo);
            _logger.LogInformation("Objetivo agregado exitosamente para Objetivo_ID: {Objetivo_ID}", dim12Objetivo.Objetivo_ID);

            _logger.LogInformation("Fin de PostDim12Objetivo para Objetivo_ID: {Objetivo_ID}", dim12Objetivo.Objetivo_ID);
            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroInsertado, objetivoId = dim12Objetivo.Objetivo_ID });
        }

        /// <summary>
        /// Modifica un Objetivo existente.
        /// </summary>
        /// <param name="id">El ID del Objetivo a modificar.</param>
        /// <param name="updateRequest">Los detalles de la actualización.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPut("{id}/modifica")]
        public async Task<IActionResult> PutDim12Objetivo(string id, [FromBody] Dim12ObjetivoUpdateRequest updateRequest, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PutDim12Objetivo para Objetivo_ID: {Objetivo_ID}", id);

            var existingObjetivo = await _dimGenericRepository.GetByIdAsync(id);
            if (existingObjetivo == null)
            {
                _logger.LogWarning("Registro no encontrado para Objetivo_ID: {Objetivo_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            existingObjetivo.EspeciesObjetivo = updateRequest.EspeciesObjetivo ?? existingObjetivo.EspeciesObjetivo;

            _logger.LogInformation("Modificando Objetivo para Objetivo_ID: {Objetivo_ID}", id);
            await _dimGenericRepository.UpdateAsync(existingObjetivo, usuarioSigo);
            _logger.LogInformation("Objetivo modificado exitosamente para Objetivo_ID: {Objetivo_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroActualizado, objetivoId = id });
        }

        /// <summary>
        /// Cambia el estado de un Objetivo (Activo/Inactivo).
        /// </summary>
        /// <param name="id">El ID del Objetivo a modificar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpDelete("{id}/cambia-estado")]
        public async Task<IActionResult> DeleteDim12Objetivo(string id, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de DeleteDim12Objetivo para Objetivo_ID: {Objetivo_ID}", id);

            var existingObjetivo = await _dimGenericRepository.GetByIdAsync(id);
            if (existingObjetivo == null)
            {
                _logger.LogWarning("Registro no encontrado para Objetivo_ID: {Objetivo_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            _logger.LogInformation("Cambiando estado de Objetivo para Objetivo_ID: {Objetivo_ID}", id);
            await _dimGenericRepository.DeleteAsync(id, usuarioSigo);
            _logger.LogInformation("Estado de Objetivo cambiado exitosamente para Objetivo_ID: {Objetivo_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.CambioEstado, objetivoId = id, nuevoEstado = existingObjetivo.Estado });
        }
    }
}