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
    public class Dim04OperatividadController : ControllerBase
    {
        private readonly IDimGenericRepository<Dim_04_Operatividad> _dimGenericRepository;
        private readonly ILogger<Dim04OperatividadController> _logger;

        public Dim04OperatividadController(IDimGenericRepository<Dim_04_Operatividad> dimGenericRepository, ILogger<Dim04OperatividadController> logger)
        {
            _dimGenericRepository = dimGenericRepository;
            _logger = logger;
        }

        /// <summary>
        /// Agrega nueva Operatividad.
        /// </summary>
        /// <param name="dim04Operatividad">Los detalles de la Operatividad a agregar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("nuevo")]
        public async Task<IActionResult> PostDim04Operatividad([FromBody] Dim_04_Operatividad dim04Operatividad, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PostDim04Operatividad para Operatividad_ID: {Operatividad_ID}", dim04Operatividad.Operatividad_ID);

            if (string.IsNullOrEmpty(dim04Operatividad.Operatividad_ID))
            {
                _logger.LogWarning("Datos inválidos para Operatividad_ID: {Operatividad_ID}", dim04Operatividad.Operatividad_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            if (await _dimGenericRepository.ExistsAsync(dim04Operatividad.Operatividad_ID))
            {
                _logger.LogWarning("Registro duplicado para Operatividad_ID: {Operatividad_ID}", dim04Operatividad.Operatividad_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.RegistroDuplicado });
            }

            _logger.LogInformation("Agregando nueva Operatividad para Operatividad_ID: {Operatividad_ID}", dim04Operatividad.Operatividad_ID);
            await _dimGenericRepository.AddAsync(dim04Operatividad, usuarioSigo);
            _logger.LogInformation("Operatividad agregada exitosamente para Operatividad_ID: {Operatividad_ID}", dim04Operatividad.Operatividad_ID);

            _logger.LogInformation("Fin de PostDim04Operatividad para Operatividad_ID: {Operatividad_ID}", dim04Operatividad.Operatividad_ID);
            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroInsertado, operatividadId = dim04Operatividad.Operatividad_ID });
        }

        /// <summary>
        /// Modifica Operatividad existente.
        /// </summary>
        /// <param name="id">El ID de la Operatividad a modificar.</param>
        /// <param name="updateRequest">Los detalles de la actualización.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPut("{id}/modifica")]
        public async Task<IActionResult> PutDim04Operatividad(string id, [FromBody] Dim04OperatividadUpdateRequest updateRequest, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PutDim04Operatividad para Operatividad_ID: {Operatividad_ID}", id);

            var existingOperatividad = await _dimGenericRepository.GetByIdAsync(id);
            if (existingOperatividad == null)
            {
                _logger.LogWarning("Registro no encontrado para Operatividad_ID: {Operatividad_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            existingOperatividad.DetalleOperatividad = updateRequest.DetalleOperatividad ?? existingOperatividad.DetalleOperatividad;
            existingOperatividad.DescripcionOperatividad = updateRequest.DescripcionOperatividad ?? existingOperatividad.DescripcionOperatividad;
            existingOperatividad.TipoOperatividad = updateRequest.TipoOperatividad ?? existingOperatividad.TipoOperatividad;

            _logger.LogInformation("Modificando Operatividad para Operatividad_ID: {Operatividad_ID}", id);
            await _dimGenericRepository.UpdateAsync(existingOperatividad, usuarioSigo);
            _logger.LogInformation("Operatividad modificada exitosamente para Operatividad_ID: {Operatividad_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroActualizado, operatividadId = id });
        }

        /// <summary>
        /// Cambia Estado de Operatividad existente (Activo/Inactivo).
        /// </summary>
        /// <param name="id">El ID de la Operatividad a modificar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpDelete("{id}/cambia-estado")]
        public async Task<IActionResult> DeleteDim04Operatividad(string id, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de DeleteDim04Operatividad para Operatividad_ID: {Operatividad_ID}", id);

            var existingOperatividad = await _dimGenericRepository.GetByIdAsync(id);
            if (existingOperatividad == null)
            {
                _logger.LogWarning("Registro no encontrado para Operatividad_ID: {Operatividad_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            _logger.LogInformation("Cambiando estado de Operatividad para Operatividad_ID: {Operatividad_ID}", id);
            await _dimGenericRepository.DeleteAsync(id, usuarioSigo);
            _logger.LogInformation("Estado de Operatividad cambiado exitosamente para Operatividad_ID: {Operatividad_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.CambioEstado, operatividadId = id, nuevoEstado = existingOperatividad.Estado });
        }
    }
}