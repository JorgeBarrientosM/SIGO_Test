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
    public class Dim10PreciosController : ControllerBase
    {
        private readonly IDimGenericRepository<Dim_10_Precios> _dimGenericRepository;
        private readonly ILogger<Dim10PreciosController> _logger;

        public Dim10PreciosController(IDimGenericRepository<Dim_10_Precios> dimGenericRepository,  ILogger<Dim10PreciosController> logger)
        {
            _dimGenericRepository = dimGenericRepository;
            _logger = logger;
        }

        /// <summary>
        /// Agrega nuevo Precio.
        /// </summary>
        /// <param name="dim10Precios">Los detalles del Precio a agregar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("nuevo")]
        public async Task<IActionResult> PostDim10Precios([FromBody] Dim_10_Precios dim10Precios, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PostDim10Precios para Precio_ID: {Precio_ID}", dim10Precios.Precio_ID);

            if (string.IsNullOrEmpty(dim10Precios.Precio_ID))
            {
                _logger.LogWarning("Datos inválidos para Precio_ID: {Precio_ID}", dim10Precios.Precio_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            if (await _dimGenericRepository.ExistsAsync(dim10Precios.Precio_ID))
            {
                _logger.LogWarning("Registro duplicado para Precio_ID: {Precio_ID}", dim10Precios.Precio_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.RegistroDuplicado });
            }

            _logger.LogInformation("Agregando nuevo Precio para Precio_ID: {Precio_ID}", dim10Precios.Precio_ID);
            await _dimGenericRepository.AddAsync(dim10Precios, usuarioSigo);
            _logger.LogInformation("Precio agregado exitosamente para Precio_ID: {Precio_ID}", dim10Precios.Precio_ID);

            _logger.LogInformation("Fin de PostDim10Precios para Precio_ID: {Precio_ID}", dim10Precios.Precio_ID);
            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroInsertado, precioId = dim10Precios.Precio_ID });
        }

        /// <summary>
        /// Modifica Precio existente.
        /// </summary>
        /// <param name="id">El ID del Precio a modificar.</param>
        /// <param name="updateRequest">Los detalles de la actualización.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPut("{id}/modifica")]
        public async Task<IActionResult> PutDim10Precios(string id, [FromBody] Dim10PreciosUpdateRequest updateRequest, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PutDim10Precios para Precio_ID: {Precio_ID}", id);

            var existingPrecio = await _dimGenericRepository.GetByIdAsync(id);
            if (existingPrecio == null)
            {
                _logger.LogWarning("Registro no encontrado para Precio_ID: {Precio_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            existingPrecio.PrecioUSD = updateRequest.PrecioUSD != -1 ? updateRequest.PrecioUSD : existingPrecio.PrecioUSD;
            existingPrecio.FechaInicial = updateRequest.FechaInicial ?? existingPrecio.FechaInicial;

            _logger.LogInformation("Modificando Precio para Precio_ID: {Precio_ID}", id);
            await _dimGenericRepository.UpdateAsync(existingPrecio, usuarioSigo);
            _logger.LogInformation("Precio modificado exitosamente para Precio_ID: {Precio_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroActualizado, precioId = id });
        }

        /// <summary>
        /// Cambia Estado de Precio existente (Activo/Inactivo).
        /// </summary>
        /// <param name="id">El ID del Precio a modificar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpDelete("{id}/cambia-estado")]
        public async Task<IActionResult> DeleteDim10Precios(string id, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de DeleteDim10Precios para Precio_ID: {Precio_ID}", id);

            var existingPrecio = await _dimGenericRepository.GetByIdAsync(id);
            if (existingPrecio == null)
            {
                _logger.LogWarning("Registro no encontrado para Precio_ID: {Precio_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            _logger.LogInformation("Cambiando estado de Precio para Precio_ID: {Precio_ID}", id);
            await _dimGenericRepository.DeleteAsync(id, usuarioSigo);
            _logger.LogInformation("Estado de Precio cambiado exitosamente para Precio_ID: {Precio_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.CambioEstado, precioId = id, nuevoEstado = existingPrecio.Estado });
        }
    }
}