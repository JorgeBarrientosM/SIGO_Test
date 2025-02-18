using Microsoft.AspNetCore.Mvc;
using BackEnd.Data;
using BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using BackEnd.Interfaces;
using BackEnd.Filters;

namespace BackEnd.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ValidateUserFilter))]
    public class Fac01MareasController : ControllerBase
    {
        private readonly ILogger<Fac01MareasController> _logger;
        private readonly IFac01Repository _fac01Repository;

        public Fac01MareasController(ILogger<Fac01MareasController> logger, IFac01Repository fac01Repository)
        {
            _logger = logger;
            _fac01Repository = fac01Repository;
        }

        /// <summary>
        /// Agrega una nueva Marea.
        /// </summary>
        /// <param name="createRequest">Los detalles de la Marea a agregar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>

        // POST: api/Fac01Mareas para agregar nueva Marea
        [HttpPost("nuevo")]
        public async Task<IActionResult> PostFac01Mareas([FromBody] CreacionMareaRequest createRequest, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PostFac01Mareas para CcoId: {CcoId}, NumMarea: {NumMarea}", createRequest.CcoId, createRequest.NumMarea);

            if (string.IsNullOrEmpty(createRequest.CcoId) || string.IsNullOrEmpty(createRequest.ObjetivoId) || string.IsNullOrEmpty(createRequest.ZonaId))
            {
                _logger.LogWarning("Datos inválidos para CcoId: {CcoId}, ObjetivoId: {ObjetivoId}, ZonaId: {ZonaId}", createRequest.CcoId, createRequest.ObjetivoId, createRequest.ZonaId);
                return BadRequest(new { codigoEstado = 0, mensaje = "Datos inválidos." });
            }

            var year = createRequest.FechaInicio.Year;
            var numMareaExists = await _fac01Repository.ExistsByDetailsAsync(createRequest.CcoId, createRequest.NumMarea, year);
            if (numMareaExists)
            {
                _logger.LogWarning("El NumMarea {NumMarea} ya existe para el mismo año y barco {CcoId}.", createRequest.NumMarea, createRequest.CcoId);
                return BadRequest(new { codigoEstado = 0, mensaje = "El NumMarea ya existe para el mismo año y barco." });
            }

            var mareaEnCursoExists = await _fac01Repository.ExistsInProgressAsync(createRequest.CcoId);
            if (mareaEnCursoExists)
            {
                _logger.LogWarning("Ya existe una marea 'En Curso' para el barco {CcoId}.", createRequest.CcoId);
                return BadRequest(new { codigoEstado = 0, mensaje = "Ya existe una marea 'En Curso' para el mismo barco." });
            }

            var mareaId = $"M-{createRequest.CcoId}-{year}{createRequest.NumMarea:D2}";

            var nuevaMarea = new Fac_01_Mareas
            {
                Marea_ID = mareaId,
                Cco_ID = createRequest.CcoId,
                NumMarea = createRequest.NumMarea,
                FechaInicio = createRequest.FechaInicio,
                FechaFinal = null,
                Objetivo_ID = createRequest.ObjetivoId,
                Zona_ID = createRequest.ZonaId,
                EstadoOperativo = "Inactiva",
                Estado = "I"
            };

            _logger.LogInformation("Agregando nueva marea con ID: {MareaId}", mareaId);
            await _fac01Repository.AddAsync(nuevaMarea, usuarioSigo);

            _logger.LogInformation("Marea creada exitosamente con ID: {MareaId}", mareaId);
            return Ok(new { codigoEstado = 1, mensaje = "Marea creada exitosamente.", mareaId = nuevaMarea.Marea_ID });
        }

        /// <summary>
        /// Activa una Marea existente.
        /// </summary>
        /// <param name="id">El ID de la Marea a activar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>

        // POST: api/Fac01Mareas/{id}/activate para activar una Marea
        [HttpPost("{id}/activar")]
        public async Task<IActionResult> ActivateMarea(string id, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de ActivateMarea para Marea_ID: {MareaId}", id);

            var marea = await _fac01Repository.GetByIdAsync(id);
            if (marea == null)
            {
                _logger.LogWarning("No se encontró la marea especificada con ID: {MareaId}", id);
                return NotFound(new { codigoEstado = 0, mensaje = "No se encontró la marea especificada." });
            }

            if (marea.EstadoOperativo != "Inactiva")
            {
                _logger.LogWarning("Solo se pueden activar mareas que están 'Inactivas'. Marea_ID: {MareaId}", id);
                return BadRequest(new { codigoEstado = 0, mensaje = "Solo se pueden activar mareas que están 'Inactivas'." });
            }

            marea.EstadoOperativo = "en Curso";
            marea.Estado = "A";
            _logger.LogInformation("Activando marea con ID: {MareaId}", id);
            await _fac01Repository.UpdateAsync(marea, usuarioSigo);

            _logger.LogInformation("Marea activada exitosamente con ID: {MareaId}", id);
            return Ok(new { codigoEstado = 1, mensaje = "Marea activada exitosamente.", mareaId = marea.Marea_ID });
        }

        /// <summary>
        /// Finaliza una Marea existente.
        /// </summary>
        /// <param name="id">El ID de la Marea a finalizar.</param>
        /// <param name="fechaFinal">La fecha de finalización de la Marea.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>

        // POST: api/Fac01Mareas/{id}/finalize para finalizar una Marea
        [HttpPost("{id}/finalizar")]
        public async Task<IActionResult> FinalizeMarea(string id, [FromQuery] DateTime fechaFinal, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de FinalizeMarea para Marea_ID: {MareaId}", id);

            var marea = await _fac01Repository.GetByIdAsync(id);
            if (marea == null)
            {
                _logger.LogWarning("No se encontró la marea especificada con ID: {MareaId}", id);
                return NotFound(new { codigoEstado = 0, mensaje = "No se encontró la marea especificada." });
            }

            if (fechaFinal <= marea.FechaInicio || fechaFinal > DateTime.Now)
            {
                _logger.LogWarning("La FechaFinal debe ser mayor que FechaInicio y menor o igual a la fecha actual. Marea_ID: {MareaId}", id);
                return BadRequest(new { codigoEstado = 0, mensaje = "La FechaFinal debe ser mayor que FechaInicio y menor o igual a la fecha actual." });
            }

            marea.FechaFinal = fechaFinal;
            marea.EstadoOperativo = "en Descarga";
            _logger.LogInformation("Finalizando marea con ID: {MareaId}", id);
            await _fac01Repository.UpdateAsync(marea, usuarioSigo);

            _logger.LogInformation("Marea finalizada exitosamente con ID: {MareaId}", id);
            return Ok(new { codigoEstado = 1, mensaje = "Marea finalizada exitosamente.", mareaId = marea.Marea_ID });
        }
    }
}