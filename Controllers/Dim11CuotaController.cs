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
    public class Dim11CuotaController : ControllerBase
    {
        private readonly IDimGenericRepository<Dim_11_Cuota> _dimGenericRepository;
        private readonly ILogger<Dim11CuotaController> _logger;

        public Dim11CuotaController(IDimGenericRepository<Dim_11_Cuota> dimGenericRepository,  ILogger<Dim11CuotaController> logger)
        {
            _dimGenericRepository = dimGenericRepository;
            _logger = logger;
        }

        /// <summary>
        /// Agrega nueva Cuota.
        /// </summary>
        /// <param name="dim11Cuota">Los detalles de la Cuota a agregar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("nuevo")]
        public async Task<IActionResult> PostDim11Cuota([FromBody] Dim_11_Cuota dim11Cuota, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PostDim11Cuota para Cuota_ID: {Cuota_ID}", dim11Cuota.Cuota_ID);

            if (string.IsNullOrEmpty(dim11Cuota.Cuota_ID))
            {
                _logger.LogWarning("Datos inválidos para Cuota_ID: {Cuota_ID}", dim11Cuota.Cuota_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            if (await _dimGenericRepository.ExistsAsync(dim11Cuota.Cuota_ID))
            {
                _logger.LogWarning("Registro duplicado para Cuota_ID: {Cuota_ID}", dim11Cuota.Cuota_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.RegistroDuplicado });
            }

            _logger.LogInformation("Agregando nueva Cuota para Cuota_ID: {Cuota_ID}", dim11Cuota.Cuota_ID);
            await _dimGenericRepository.AddAsync(dim11Cuota, usuarioSigo);
            _logger.LogInformation("Cuota agregada exitosamente para Cuota_ID: {Cuota_ID}", dim11Cuota.Cuota_ID);

            _logger.LogInformation("Fin de PostDim11Cuota para Cuota_ID: {Cuota_ID}", dim11Cuota.Cuota_ID);
            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroInsertado, cuotaId = dim11Cuota.Cuota_ID });
        }

        /// <summary>
        /// Modifica Cuota existente.
        /// </summary>
        /// <param name="id">El ID de la Cuota a modificar.</param>
        /// <param name="updateRequest">Los detalles de la actualización.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPut("{id}/modifica")]
        public async Task<IActionResult> PutDim11Cuota(string id, [FromBody] Dim11CuotaUpdateRequest updateRequest, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PutDim11Cuota para Cuota_ID: {Cuota_ID}", id);

            var existingCuota = await _dimGenericRepository.GetByIdAsync(id);
            if (existingCuota == null)
            {
                _logger.LogWarning("Registro no encontrado para Cuota_ID: {Cuota_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            existingCuota.DescripcionMvtoCuota = updateRequest.DescripcionMvtoCuota ?? existingCuota.DescripcionMvtoCuota;
            existingCuota.TipoCuota = updateRequest.TipoCuota ?? existingCuota.TipoCuota;
            existingCuota.Tratamiento = updateRequest.Tratamiento ?? existingCuota.Tratamiento;

            _logger.LogInformation("Modificando Cuota para Cuota_ID: {Cuota_ID}", id);
            await _dimGenericRepository.UpdateAsync(existingCuota, usuarioSigo);
            _logger.LogInformation("Cuota modificada exitosamente para Cuota_ID: {Cuota_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroActualizado, cuotaId = id });
        }

        /// <summary>
        /// Cambia Estado de Cuota existente (Activo/Inactivo).
        /// </summary>
        /// <param name="id">El ID de la Cuota a modificar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpDelete("{id}/cambia-estado")]
        public async Task<IActionResult> DeleteDim11Cuota(string id, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de DeleteDim11Cuota para Cuota_ID: {Cuota_ID}", id);

            var existingCuota = await _dimGenericRepository.GetByIdAsync(id);
            if (existingCuota == null)
            {
                _logger.LogWarning("Registro no encontrado para Cuota_ID: {Cuota_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            _logger.LogInformation("Cambiando estado de Cuota para Cuota_ID: {Cuota_ID}", id);
            await _dimGenericRepository.DeleteAsync(id, usuarioSigo);
            _logger.LogInformation("Estado de Cuota cambiado exitosamente para Cuota_ID: {Cuota_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.CambioEstado, cuotaId = id, nuevoEstado = existingCuota.Estado });
        }
    }
}