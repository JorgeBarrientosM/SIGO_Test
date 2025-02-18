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
    public class Dim03BarcosController : ControllerBase
    {
        private readonly IDimGenericRepository<Dim_03_Barcos> _dimGenericRepository;
        private readonly ILogger<Dim03BarcosController> _logger;

        public Dim03BarcosController(IDimGenericRepository<Dim_03_Barcos> dimGenericRepository, ILogger<Dim03BarcosController> logger)
        {
            _dimGenericRepository = dimGenericRepository;
            _logger = logger;
        }

        /// <summary>
        /// Agrega nuevo Barco.
        /// </summary>
        /// <param name="dim03Barcos">Los detalles del Barco a agregar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("nuevo")]
        public async Task<IActionResult> PostDim03Barcos([FromBody] Dim_03_Barcos dim03Barcos, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PostDim03Barcos para Cco_ID: {Cco_ID}, Matricula: {Matricula}", dim03Barcos.Cco_ID, dim03Barcos.Matricula);

            if (string.IsNullOrEmpty(dim03Barcos.Cco_ID) || string.IsNullOrEmpty(dim03Barcos.Matricula))
            {
                _logger.LogWarning("Datos inválidos para Cco_ID: {Cco_ID}, Matricula: {Matricula}", dim03Barcos.Cco_ID, dim03Barcos.Matricula);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            if (await _dimGenericRepository.ExistsAsync(dim03Barcos.Cco_ID))
            {
                _logger.LogWarning("Registro duplicado para Cco_ID: {Cco_ID}", dim03Barcos.Cco_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.RegistroDuplicado });
            }

            _logger.LogInformation("Agregando nuevo Barco para Cco_ID: {Cco_ID}, Matricula: {Matricula}", dim03Barcos.Cco_ID, dim03Barcos.Matricula);
            await _dimGenericRepository.AddAsync(dim03Barcos, usuarioSigo);
            _logger.LogInformation("Barco agregado exitosamente para Cco_ID: {Cco_ID}, Matricula: {Matricula}", dim03Barcos.Cco_ID, dim03Barcos.Matricula);

            _logger.LogInformation("Fin de PostDim03Barcos para Cco_ID: {Cco_ID}, Matricula: {Matricula}", dim03Barcos.Cco_ID, dim03Barcos.Matricula);
            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroInsertado, ccoId = dim03Barcos.Cco_ID });
        }

        /// <summary>
        /// Modifica Barco existente.
        /// </summary>
        /// <param name="id">El ID del Barco a modificar.</param>
        /// <param name="updateRequest">Los detalles de la actualización.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPut("{id}/modifica")]
        public async Task<IActionResult> PutDim03Barcos(string id, [FromBody] Dim03BarcosUpdateRequest updateRequest, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PutDim03Barcos para Cco_ID: {Cco_ID}", id);

            var existingBarco = await _dimGenericRepository.GetByIdAsync(id);
            if (existingBarco == null)
            {
                _logger.LogWarning("Registro no encontrado para Cco_ID: {Cco_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            existingBarco.ArtePesca_ID = updateRequest.ArtePesca_ID ?? existingBarco.ArtePesca_ID;
            existingBarco.Matricula = updateRequest.Matricula ?? existingBarco.Matricula;
            existingBarco.Año = updateRequest.Año != -1 ? updateRequest.Año : existingBarco.Año;
            existingBarco.TRG = updateRequest.TRG != -1 ? updateRequest.TRG : existingBarco.TRG;
            existingBarco.Eslora = updateRequest.Eslora != -1 ? updateRequest.Eslora : existingBarco.Eslora;
            existingBarco.Manga = updateRequest.Manga != -1 ? updateRequest.Manga : existingBarco.Manga;
            existingBarco.CapProduccion = updateRequest.CapProduccion != -1 ? updateRequest.CapProduccion : existingBarco.CapProduccion;
            existingBarco.CapMaxPetroleo = updateRequest.CapMaxPetroleo != -1 ? updateRequest.CapMaxPetroleo : existingBarco.CapMaxPetroleo;
            existingBarco.VelocidadMaxima = updateRequest.VelocidadMaxima != -1 ? updateRequest.VelocidadMaxima : existingBarco.VelocidadMaxima;
            existingBarco.Tripulacion = updateRequest.Tripulacion != -1 ? updateRequest.Tripulacion : existingBarco.Tripulacion;
            existingBarco.ConsumoMaximo = updateRequest.ConsumoMaximo != -1 ? updateRequest.ConsumoMaximo : existingBarco.ConsumoMaximo;

            _logger.LogInformation("Modificando Barco para Cco_ID: {Cco_ID}", id);
            await _dimGenericRepository.UpdateAsync(existingBarco, usuarioSigo);
            _logger.LogInformation("Barco modificado exitosamente para Cco_ID: {Cco_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.RegistroActualizado, ccoId = id });
        }

        /// <summary>
        /// Cambia Estado de Barco existente (Activo/Inactivo).
        /// </summary>
        /// <param name="id">El ID del Barco a modificar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpDelete("{id}/cambia-estado")]
        public async Task<IActionResult> DeleteDim03Barcos(string id, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de DeleteDim03Barcos para Cco_ID: {Cco_ID}", id);

            var existingBarco = await _dimGenericRepository.GetByIdAsync(id);
            if (existingBarco == null)
            {
                _logger.LogWarning("Registro no encontrado para Cco_ID: {Cco_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            _logger.LogInformation("Cambiando estado de Barco para Cco_ID: {Cco_ID}", id);
            await _dimGenericRepository.DeleteAsync(id, usuarioSigo);
            _logger.LogInformation("Estado de Barco cambiado exitosamente para Cco_ID: {Cco_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.CambioEstado, ccoId = id, nuevoEstado = existingBarco.Estado });
        }
    }
}