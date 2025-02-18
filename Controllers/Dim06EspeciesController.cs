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
    public class Dim06EspeciesController : ControllerBase
    {
        private readonly IDimGenericRepository<Dim_06_Especies> _dimGenericRepository;
        private readonly ILogger<Dim06EspeciesController> _logger;

        public Dim06EspeciesController(IDimGenericRepository<Dim_06_Especies> dimGenericRepository, ILogger<Dim06EspeciesController> logger)
        {
            _dimGenericRepository = dimGenericRepository;
            _logger = logger;
        }

        /// <summary>
        /// Agrega nueva Especie.
        /// </summary>
        /// <param name="dim06Especies">Los detalles de la Especie a agregar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("nuevo")]
        public async Task<IActionResult> PostDim06Especies([FromBody] Dim_06_Especies dim06Especies, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PostDim06Especies para Especie_ID: {Especie_ID}", dim06Especies.Especie_ID);

            if (string.IsNullOrEmpty(dim06Especies.Especie_ID))
            {
                _logger.LogWarning("Datos inválidos para Especie_ID: {Especie_ID}", dim06Especies.Especie_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            if (await _dimGenericRepository.ExistsAsync(dim06Especies.Especie_ID))
            {
                _logger.LogWarning("Registro duplicado para Especie_ID: {Especie_ID}", dim06Especies.Especie_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.RegistroDuplicado });
            }

            _logger.LogInformation("Agregando nueva Especie para Especie_ID: {Especie_ID}", dim06Especies.Especie_ID);
            await _dimGenericRepository.AddAsync(dim06Especies, usuarioSigo);
            _logger.LogInformation("Especie agregada exitosamente para Especie_ID: {Especie_ID}", dim06Especies.Especie_ID);

            _logger.LogInformation("Fin de PostDim06Especies para Especie_ID: {Especie_ID}", dim06Especies.Especie_ID);
            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroInsertado, especieId = dim06Especies.Especie_ID });
        }

        /// <summary>
        /// Modifica Especie existente.
        /// </summary>
        /// <param name="id">El ID de la Especie a modificar.</param>
        /// <param name="updateRequest">Los detalles de la actualización.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPut("{id}/modifica")]
        public async Task<IActionResult> PutDim06Especies(string id, [FromBody] Dim06EspeciesUpdateRequest updateRequest, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PutDim06Especies para Especie_ID: {Especie_ID}", id);

            var existingEspecie = await _dimGenericRepository.GetByIdAsync(id);
            if (existingEspecie == null)
            {
                _logger.LogWarning("Registro no encontrado para Especie_ID: {Especie_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            existingEspecie.DescripcionEspecie = updateRequest.DescripcionEspecie ?? existingEspecie.DescripcionEspecie;
            existingEspecie.GrupoEspecie = updateRequest.GrupoEspecie ?? existingEspecie.GrupoEspecie;

            _logger.LogInformation("Modificando Especie para Especie_ID: {Especie_ID}", id);
            await _dimGenericRepository.UpdateAsync(existingEspecie, usuarioSigo);
            _logger.LogInformation("Especie modificada exitosamente para Especie_ID: {Especie_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroActualizado, especieId = id });
        }

        /// <summary>
        /// Cambia Estado de Especie existente (Activo/Inactivo).
        /// </summary>
        /// <param name="id">El ID de la Especie a modificar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpDelete("{id}/cambia-estado")]
        public async Task<IActionResult> DeleteDim06Especies(string id, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de DeleteDim06Especies para Especie_ID: {Especie_ID}", id);

            var existingEspecie = await _dimGenericRepository.GetByIdAsync(id);
            if (existingEspecie == null)
            {
                _logger.LogWarning("Registro no encontrado para Especie_ID: {Especie_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            _logger.LogInformation("Cambiando estado de Especie para Especie_ID: {Especie_ID}", id);
            await _dimGenericRepository.DeleteAsync(id, usuarioSigo);
            _logger.LogInformation("Estado de Especie cambiado exitosamente para Especie_ID: {Especie_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.CambioEstado, especieId = id, nuevoEstado = existingEspecie.Estado });
        }
    }
}