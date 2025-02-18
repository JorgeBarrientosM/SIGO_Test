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
    public class Dim09ProductosController : ControllerBase
    {
        private readonly IDimGenericRepository<Dim_09_Productos> _dimGenericRepository;
        private readonly ILogger<Dim09ProductosController> _logger;

        public Dim09ProductosController(IDimGenericRepository<Dim_09_Productos> dimGenericRepository, ILogger<Dim09ProductosController> logger)
        {
            _dimGenericRepository = dimGenericRepository;
            _logger = logger;
        }

        /// <summary>
        /// Agrega nuevo Producto.
        /// </summary>
        /// <param name="dim09Productos">Los detalles del Producto a agregar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("nuevo")]
        public async Task<IActionResult> PostDim09Productos([FromBody] Dim_09_Productos dim09Productos, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PostDim09Productos para Producto_ID: {Producto_ID}", dim09Productos.Producto_ID);

            if (string.IsNullOrEmpty(dim09Productos.Producto_ID))
            {
                _logger.LogWarning("Datos inválidos para Producto_ID: {Producto_ID}", dim09Productos.Producto_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            if (await _dimGenericRepository.ExistsAsync(dim09Productos.Producto_ID))
            {
                _logger.LogWarning("Registro duplicado para Producto_ID: {Producto_ID}", dim09Productos.Producto_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.RegistroDuplicado });
            }

            _logger.LogInformation("Agregando nuevo Producto para Producto_ID: {Producto_ID}", dim09Productos.Producto_ID);
            await _dimGenericRepository.AddAsync(dim09Productos, usuarioSigo);
            _logger.LogInformation("Producto agregado exitosamente para Producto_ID: {Producto_ID}", dim09Productos.Producto_ID);

            _logger.LogInformation("Fin de PostDim09Productos para Producto_ID: {Producto_ID}", dim09Productos.Producto_ID);
            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroInsertado, poductoId = dim09Productos.Producto_ID });
        }

        /// <summary>
        /// Modifica Producto existente.
        /// </summary>
        /// <param name="id">El ID del Producto a modificar.</param>
        /// <param name="updateRequest">Los detalles de la actualización.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPut("{id}/modifica")]
        public async Task<IActionResult> PutDim09Productos(string id, [FromBody] Dim09ProductosUpdateRequest updateRequest, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PutDim09Productos para Producto_ID: {Producto_ID}", id);

            var existingProducto = await _dimGenericRepository.GetByIdAsync(id);
            if (existingProducto == null)
            {
                _logger.LogWarning("Registro no encontrado para Producto_ID: {Producto_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            existingProducto.CodigoJDE = updateRequest.CodigoJDE ?? existingProducto.CodigoJDE;
            existingProducto.Factor = updateRequest.Factor != -1 ? updateRequest.Factor : existingProducto.Factor;

            _logger.LogInformation("Modificando Producto para Producto_ID: {Producto_ID}", id);
            await _dimGenericRepository.UpdateAsync(existingProducto, usuarioSigo);
            _logger.LogInformation("Producto modificado exitosamente para Producto_ID: {Producto_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroActualizado, poductoId = id });
        }

        /// <summary>
        /// Cambia Estado de Producto existente (Activo/Inactivo).
        /// </summary>
        /// <param name="id">El ID del Producto a modificar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpDelete("{id}/cambia-estado")]
        public async Task<IActionResult> DeleteDim09Productos(string id, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de DeleteDim09Productos para Producto_ID: {Producto_ID}", id);

            var existingProducto = await _dimGenericRepository.GetByIdAsync(id);
            if (existingProducto == null)
            {
                _logger.LogWarning("Registro no encontrado para Producto_ID: {Producto_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            _logger.LogInformation("Cambiando estado de Producto para Producto_ID: {Producto_ID}", id);
            await _dimGenericRepository.DeleteAsync(id, usuarioSigo);
            _logger.LogInformation("Estado de Producto cambiado exitosamente para Producto_ID: {Producto_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.CambioEstado, poductoId = id, nuevoEstado = existingProducto.Estado });
        }
    }
}