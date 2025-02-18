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
    public class Dim02ArtePescaController : ControllerBase
    {
        private readonly IDimGenericRepository<Dim_02_ArtePesca> _dimGenericRepository;
        private readonly ILogger<Dim02ArtePescaController> _logger;

        public Dim02ArtePescaController(IDimGenericRepository<Dim_02_ArtePesca> dimGenericRepository, ILogger<Dim02ArtePescaController> logger)
        {
            _dimGenericRepository = dimGenericRepository;
            _logger = logger;
        }

        /// <summary>
        /// Agrega nuevo Arte de Pesca.
        /// </summary>
        /// <param name="dim02ArtePesca">Los detalles del Arte de Pesca a agregar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("nuevo")]
        public async Task<IActionResult> PostDim02ArtePesca([FromBody] Dim_02_ArtePesca dim02ArtePesca, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PostDim02ArtePesca para ArtePesca:ID: {ArtePesca_ID}", dim02ArtePesca.ArtePesca_ID);

            if (string.IsNullOrEmpty(dim02ArtePesca.ArtePesca_ID))
            {
                _logger.LogWarning("Datos inválidos para ArtePesca_ID: {Artepesca_ID}", dim02ArtePesca.ArtePesca_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos});
            }

            if (await _dimGenericRepository.ExistsAsync(dim02ArtePesca.ArtePesca_ID))
            {
                _logger.LogWarning("Registro duplicado para ArtePesca_ID: {ArtePesca_ID}", dim02ArtePesca.ArtePesca_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.RegistroDuplicado});
            }

            _logger.LogInformation("Agregando nuevo Arte de Pesca para ArtePesca_ID: {ArtePesca_ID}", dim02ArtePesca.ArtePesca_ID);
            await _dimGenericRepository.AddAsync(dim02ArtePesca, usuarioSigo);
            _logger.LogInformation("Arte de Pesca agregado exitosamente para ArtePesca_ID: {Artepesca_ID}", dim02ArtePesca.ArtePesca_ID);

            _logger.LogInformation("Fin de PostDim02ArtePesca para ArtePesca_ID: {ArtePesca_ID}", dim02ArtePesca.ArtePesca_ID);
            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroInsertado, artePescaId = dim02ArtePesca.ArtePesca_ID});
        }

        /// <summary>
        /// Modifica Arte de Pesca existente.
        /// </summary>
        /// <param name="id">El ID del Arte de Pesca a modificar.</param>
        /// <param name="updateRequest">Los detalles de la actualización.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPut("{id}/modifica")]
        public async Task<IActionResult> PutDim02ArtePesca(string id, [FromBody] Dim02ArtePescaUpdateRequest updateRequest, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PutDim02ArtePesca para ArtePesca_ID: {ArtePesca_ID}", id);

            var existingArtePesca = await _dimGenericRepository.GetByIdAsync(id);
            if (existingArtePesca == null)
            {
                _logger.LogWarning("Registro no encontrado para ArtePesca_ID: {ArtePesca_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste});
            }

            existingArtePesca.DescripcionArtePesca = updateRequest.DescripcionArtePesca ?? existingArtePesca.DescripcionArtePesca;

            _logger.LogInformation("Modificando Arte de Pesca para ArtePesca_ID: {ArtePesca_ID}", id);
            await _dimGenericRepository.UpdateAsync(existingArtePesca, usuarioSigo);
            _logger.LogInformation("Arte de Pesca modificado exitosamente para ArtePesca_ID: {ArtePesca_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroActualizado, artePescaId = id });
        }

        /// <summary>
        /// Cambia Estado de Arte de Pesca existente (Activo/Inactivo).
        /// </summary>
        /// <param name="id">El ID del Arte de Pesca a modificar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpDelete("{id}/cambia-estado")]
        public async Task<IActionResult> DeleteDim02ArtePesca(string id, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de DeleteDim02ArtePesca para ArtePesca_ID: {ArtePesca_ID}", id);

            var existingArtePesca = await _dimGenericRepository.GetByIdAsync(id);
            if (existingArtePesca == null)
            {
                _logger.LogWarning("Registro no encontrado para ArtePesca_ID: {ArtePesca_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste});
            }

            _logger.LogInformation("Cambiando estado de Arte de Pesca para ArtePesca_ID: {ArtePesca_ID}", id);
            await _dimGenericRepository.DeleteAsync(id, usuarioSigo);
            _logger.LogInformation("Estado de Arte de Pesca cambiado exitosamente para ArtePesca_ID: {ArtePesca_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.CambioEstado, artePescaId = id, nuevoEstado = existingArtePesca.Estado });
        }
    }
}