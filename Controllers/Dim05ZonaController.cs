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
    public class Dim05ZonaController : ControllerBase
    {
        private readonly IDimGenericRepository<Dim_05_Zona> _dimGenericRepository;
        private readonly ILogger<Dim05ZonaController> _logger;

        public Dim05ZonaController(IDimGenericRepository<Dim_05_Zona> dimGenericRepository,  ILogger<Dim05ZonaController> logger)
        {
            _dimGenericRepository = dimGenericRepository;
            _logger = logger;
        }

        /// <summary>
        /// Agrega nueva Zona de Pesca.
        /// </summary>
        /// <param name="dim05Zona">Los detalles de la Zona a agregar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("nuevo")]
        public async Task<IActionResult> PostDim05Zona([FromBody] Dim_05_Zona dim05Zona, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PostDim05Zona para Zona_ID: {Zona_ID}", dim05Zona.Zona_ID);

            if (string.IsNullOrEmpty(dim05Zona.Zona_ID))
            {
                _logger.LogWarning("Datos inválidos para Zona_ID: {Zona_ID}", dim05Zona.Zona_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            if (await _dimGenericRepository.ExistsAsync(dim05Zona.Zona_ID))
            {
                _logger.LogWarning("Registro duplicado para Zona_ID: {Zona_ID}", dim05Zona.Zona_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.RegistroDuplicado });
            }

            _logger.LogInformation("Agregando nueva Zona de Pesca para Zona_ID: {Zona_ID}", dim05Zona.Zona_ID);
            await _dimGenericRepository.AddAsync(dim05Zona, usuarioSigo);
            _logger.LogInformation("Zona de Pesca agregada exitosamente para Zona_ID: {Zona_ID}", dim05Zona.Zona_ID);

            _logger.LogInformation("Fin de PostDim05Zona para Zona_ID: {Zona_ID}", dim05Zona.Zona_ID);
            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroInsertado, zonaId = dim05Zona.Zona_ID });
        }

        /// <summary>
        /// Modifica Zona de Pesca existente.
        /// </summary>
        /// <param name="id">El ID de la Zona a modificar.</param>
        /// <param name="updateRequest">Los detalles de la actualización.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPut("{id}/modifica")]
        public async Task<IActionResult> PutDim05Zona(string id, [FromBody] Dim05ZonaUpdateRequest updateRequest, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PutDim05Zona para Zona_ID: {Zona_ID}", id);

            var existingZona = await _dimGenericRepository.GetByIdAsync(id);
            if (existingZona == null)
            {
                _logger.LogWarning("Registro no encontrado para Zona_ID: {Zona_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            existingZona.DescripcionZona = updateRequest.DescripcionZona ?? existingZona.DescripcionZona;

            _logger.LogInformation("Modificando Zona de Pesca para Zona_ID: {Zona_ID}", id);
            await _dimGenericRepository.UpdateAsync(existingZona, usuarioSigo);
            _logger.LogInformation("Zona de Pesca modificada exitosamente para Zona_ID: {Zona_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroActualizado, zonaId = id });
        }

        /// <summary>
        /// Cambia Estado de Zona de Pesca existente (Activo/Inactivo).
        /// </summary>
        /// <param name="id">El ID de la Zona a modificar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpDelete("{id}/cambia-estado")]
        public async Task<IActionResult> DeleteDim05Zona(string id, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de DeleteDim05Zona para Zona_ID: {Zona_ID}", id);

            var existingZona = await _dimGenericRepository.GetByIdAsync(id);
            if (existingZona == null)
            {
                _logger.LogWarning("Registro no encontrado para Zona_ID: {Zona_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            _logger.LogInformation("Cambiando estado de Zona de Pesca para Zona_ID: {Zona_ID}", id);
            await _dimGenericRepository.DeleteAsync(id, usuarioSigo);
            _logger.LogInformation("Estado de Zona de Pesca cambiado exitosamente para Zona_ID: {Zona_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.CambioEstado, zonaId = id, nuevoEstado = existingZona.Estado });
        }
    }
}