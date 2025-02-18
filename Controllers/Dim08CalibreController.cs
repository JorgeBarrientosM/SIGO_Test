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
    public class Dim08CalibreController : ControllerBase
    {
        private readonly IDimGenericRepository<Dim_08_Calibre> _dimGenericRepository;
        private readonly ILogger<Dim08CalibreController> _logger;

        public Dim08CalibreController(IDimGenericRepository<Dim_08_Calibre> dimGenericRepository,  ILogger<Dim08CalibreController> logger)
        {
            _dimGenericRepository = dimGenericRepository;
            _logger = logger;
        }

        /// <summary>
        /// Agrega nuevo Calibre.
        /// </summary>
        /// <param name="dim08Calibre">Los detalles del Calibre a agregar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("nuevo")]
        public async Task<IActionResult> PostDim08Calibre([FromBody] Dim_08_Calibre dim08Calibre, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PostDim08Calibre para Calibre_ID: {Calibre_ID}", dim08Calibre.Calibre_ID);

            if (string.IsNullOrEmpty(dim08Calibre.Calibre_ID))
            {
                _logger.LogWarning("Datos inválidos para Calibre_ID: {Calibre_ID}", dim08Calibre.Calibre_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            if (await _dimGenericRepository.ExistsAsync(dim08Calibre.Calibre_ID))
            {
                _logger.LogWarning("Registro duplicado para Calibre_ID: {Calibre_ID}", dim08Calibre.Calibre_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.RegistroDuplicado });
            }

            _logger.LogInformation("Agregando nuevo Calibre para Calibre_ID: {Calibre_ID}", dim08Calibre.Calibre_ID);
            await _dimGenericRepository.AddAsync(dim08Calibre, usuarioSigo);
            _logger.LogInformation("Calibre agregado exitosamente para Calibre_ID: {Calibre_ID}", dim08Calibre.Calibre_ID);

            _logger.LogInformation("Fin de PostDim08Calibre para Calibre_ID: {Calibre_ID}", dim08Calibre.Calibre_ID);
            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroInsertado, calibreId = dim08Calibre.Calibre_ID });
        }

        /// <summary>
        /// Modifica Calibre existente.
        /// </summary>
        /// <param name="id">El ID del Calibre a modificar.</param>
        /// <param name="updateRequest">Los detalles de la actualización.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPut("{id}/modifica")]
        public async Task<IActionResult> PutDim08Calibre(string id, [FromBody] Dim08CalibreUpdateRequest updateRequest, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PutDim08Calibre para Calibre_ID: {Calibre_ID}", id);

            var existingCalibre = await _dimGenericRepository.GetByIdAsync(id);
            if (existingCalibre == null)
            {
                _logger.LogWarning("Registro no encontrado para Calibre_ID: {Calibre_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            existingCalibre.DescripcionCalibre = updateRequest.DescripcionCalibre ?? existingCalibre.DescripcionCalibre;

            _logger.LogInformation("Modificando Calibre para Calibre_ID: {Calibre_ID}", id);
            await _dimGenericRepository.UpdateAsync(existingCalibre, usuarioSigo);
            _logger.LogInformation("Calibre modificado exitosamente para Calibre_ID: {Calibre_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroActualizado, calibreId = id });
        }

        /// <summary>
        /// Cambia Estado de Calibre (Activo/Inactivo).
        /// </summary>
        /// <param name="id">El ID del Calibre a modificar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpDelete("{id}/cambia-estado")]
        public async Task<IActionResult> DeleteDim08Calibre(string id, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de DeleteDim08Calibre para Calibre_ID: {Calibre_ID}", id);

            var existingCalibre = await _dimGenericRepository.GetByIdAsync(id);
            if (existingCalibre == null)
            {
                _logger.LogWarning("Registro no encontrado para Calibre_ID: {Calibre_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            _logger.LogInformation("Cambiando estado de Calibre para Calibre_ID: {Calibre_ID}", id);
            await _dimGenericRepository.DeleteAsync(id, usuarioSigo);
            _logger.LogInformation("Estado de Calibre cambiado exitosamente para Calibre_ID: {Calibre_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.CambioEstado, calibreId = id, nuevoEstado = existingCalibre.Estado });
        }
    }
}