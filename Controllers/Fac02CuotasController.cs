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
    public class Fac02CuotasController : ControllerBase
    {
        private readonly IFac02Repository _fac02Repository;
        private readonly ILogger<Fac02CuotasController> _logger;

        public Fac02CuotasController(IFac02Repository fac02Repository, ILogger<Fac02CuotasController> logger)
        {
            _fac02Repository = fac02Repository;
            _logger = logger;
        }

        /// <summary>
        /// Agrega nuevas cuotas.
        /// </summary>
        /// <param name="request">Los detalles de la cuota a agregar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("aumenta")]
        public async Task<IActionResult> AumentoCuotas([FromBody] AumentaCuotasRequest request, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de AumentoCuotas para Cuota_ID: {Cuota_ID}, Especie_ID: {Especie_ID}", request.Cuota_ID, request.Especie_ID);

            if (string.IsNullOrEmpty(request.Cuota_ID) || string.IsNullOrEmpty(request.Especie_ID) || string.IsNullOrEmpty(request.Zona_ID))
            {
                _logger.LogWarning("Datos inválidos para Cuota_ID: {Cuota_ID}, Especie_ID: {Especie_ID}, Zona_ID: {Zona_ID}", request.Cuota_ID, request.Especie_ID, request.Zona_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = "Datos inválidos." });
            }

            var tratamiento = await _fac02Repository.GetTratamientoByCuotaIdAsync(request.Cuota_ID);
            if (tratamiento != "Aumenta")
            {
                _logger.LogWarning("El Cuota_ID {Cuota_ID} no corresponde a un movimiento de tipo Aumenta.", request.Cuota_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = "El Cuota_ID especificado no corresponde a un movimiento de tipo Aumenta." });
            }

            var tipoCuota = await _fac02Repository.GetTipoCuotaByCuotaIdAsync(request.Cuota_ID);
            if (tipoCuota == "Unica" && await _fac02Repository.ExistsCuotaUnicaAsync(request.Cuota_ID, request.Especie_ID, request.Año))
            {
                _logger.LogWarning("Cuota Propia ya existe para Cuota_ID: {Cuota_ID}, Especie_ID: {Especie_ID}, Año: {Año}", request.Cuota_ID, request.Especie_ID, request.Año);
                return BadRequest(new { codigoEstado = 0, mensaje = "Cuota Propia ya existe para esta Especie/Año, favor revisar y reintentar." });
            }

            var maxSecuencia = await _fac02Repository.GetMaxSecuenciaAsync(request.Cuota_ID, request.Especie_ID, request.Zona_ID, request.Año);
            var nuevaSecuencia = maxSecuencia + 1;

            var cuotaId = $"{request.Cuota_ID}-{request.Especie_ID}-{request.Zona_ID}-{request.Año}-{nuevaSecuencia:D3}";

            var nuevaCuota = new Fac_02_Cuotas
            {
                Control_ID = cuotaId,
                Cuota_ID = request.Cuota_ID,
                Especie_ID = request.Especie_ID,
                Año = request.Año,
                Mes = request.Mes,
                Toneladas = request.Toneladas,
                Zona_ID = request.Zona_ID,
                Secuencia = nuevaSecuencia,
                Comentario = request.Comentario
            };

            _logger.LogInformation("Agregando nueva cuota con Control_ID: {Control_ID}", cuotaId);
            await _fac02Repository.AddAsync(nuevaCuota, usuarioSigo);

            _logger.LogInformation("Cuota creada exitosamente con Control_ID: {Control_ID}", cuotaId);
            return Ok(new { codigoEstado = 1, mensaje = "Cuota creada exitosamente.", cuotaId = nuevaCuota.Control_ID });
        }

        /// <summary>
        /// Agrega traspasos de cuotas a terceros.
        /// </summary>
        /// <param name="request">Los detalles del traspaso de cuota.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("rebaja")]
        public async Task<IActionResult> TraspasoCuotas([FromBody] RebajaCuotasRequest request, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de TraspasoCuotas para Cuota_ID: {Cuota_ID}, Especie_ID: {Especie_ID}", request.Cuota_ID, request.Especie_ID);

            if (string.IsNullOrEmpty(request.Cuota_ID) || string.IsNullOrEmpty(request.Especie_ID) || string.IsNullOrEmpty(request.Zona_ID))
            {
                _logger.LogWarning("Datos inválidos para Cuota_ID: {Cuota_ID}, Especie_ID: {Especie_ID}, Zona_ID: {Zona_ID}", request.Cuota_ID, request.Especie_ID, request.Zona_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = "Datos inválidos." });
            }

            var tratamiento = await _fac02Repository.GetTratamientoByCuotaIdAsync(request.Cuota_ID);
            if (tratamiento != "Rebaja")
            {
                _logger.LogWarning("El Cuota_ID {Cuota_ID} no corresponde a un movimiento de tipo Rebaja.", request.Cuota_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = "Cuota_ID especificado no corresponde a un movimiento de tipo Rebaja." });
            }

            var maxSecuencia = await _fac02Repository.GetMaxSecuenciaAsync(request.Cuota_ID, request.Especie_ID, request.Zona_ID, request.Año);
            var nuevaSecuencia = maxSecuencia + 1;

            var cuotaId = $"{request.Cuota_ID}-{request.Especie_ID}-{request.Zona_ID}-{request.Año}-{nuevaSecuencia:D3}";

            var nuevaCuota = new Fac_02_Cuotas
            {
                Control_ID = cuotaId,
                Cuota_ID = request.Cuota_ID,
                Especie_ID = request.Especie_ID,
                Año = request.Año,
                Mes = request.Mes,
                Toneladas = -request.Toneladas,
                Zona_ID = request.Zona_ID,
                Secuencia = nuevaSecuencia,
                Comentario = request.Comentario
            };

            _logger.LogInformation("Agregando nueva cuota con Control_ID: {Control_ID}", cuotaId);
            await _fac02Repository.AddAsync(nuevaCuota, usuarioSigo);

            _logger.LogInformation("Cuota creada exitosamente con Control_ID: {Control_ID}", cuotaId);
            return Ok(new { codigoEstado = 1, mensaje = "Cuota creada exitosamente.", cuotaId = nuevaCuota.Control_ID });
        }

        /// <summary>
        /// Traspasa Cuotas entre Zonas de Pesca.
        /// </summary>
        /// <param name="request">Los detalles del cambio de zona.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("cambio-zona")]
        public async Task<IActionResult> CambioZona([FromBody] CambioZonaRequest request, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de CambioZona para Cuota_ID: {Cuota_ID}, Especie_ID: {Especie_ID}", request.Cuota_ID, request.Especie_ID);

            if (string.IsNullOrEmpty(request.Cuota_ID) || string.IsNullOrEmpty(request.Especie_ID) || string.IsNullOrEmpty(request.ZonaOrigen_ID) || string.IsNullOrEmpty(request.ZonaDestino_ID))
            {
                _logger.LogWarning("Datos inválidos para Cuota_ID: {Cuota_ID}, Especie_ID: {Especie_ID}, ZonaOrigen_ID: {ZonaOrigen_ID}, ZonaDestino_ID: {ZonaDestino_ID}", request.Cuota_ID, request.Especie_ID, request.ZonaOrigen_ID, request.ZonaDestino_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = "Datos inválidos." });
            }

            var tratamiento = await _fac02Repository.GetTratamientoByCuotaIdAsync(request.Cuota_ID);
            if (tratamiento != "Cambio Zona")
            {
                _logger.LogWarning("El Cuota_ID {Cuota_ID} no corresponde a un movimiento de tipo Cambio Zona.", request.Cuota_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = "Cuota_ID especificado no corresponde a un movimiento de tipo Cambio Zona." });
            }

            var maxSecuenciaOrigen = await _fac02Repository.GetMaxSecuenciaAsync(request.Cuota_ID, request.Especie_ID, request.ZonaOrigen_ID, request.Año);
            var nuevaSecuenciaOrigen = maxSecuenciaOrigen + 1;

            var cuotaIdOrigen = $"{request.Cuota_ID}-{request.Especie_ID}-{request.ZonaOrigen_ID}-{request.Año}-{nuevaSecuenciaOrigen:D3}";

            var cuotaOrigen = new Fac_02_Cuotas
            {
                Control_ID = cuotaIdOrigen,
                Cuota_ID = request.Cuota_ID,
                Especie_ID = request.Especie_ID,
                Año = request.Año,
                Mes = request.Mes,
                Toneladas = -request.Toneladas,
                Zona_ID = request.ZonaOrigen_ID,
                Secuencia = nuevaSecuenciaOrigen,
                Comentario = request.Comentario
            };

            var maxSecuenciaDestino = await _fac02Repository.GetMaxSecuenciaAsync(request.Cuota_ID, request.Especie_ID, request.ZonaDestino_ID, request.Año);
            var nuevaSecuenciaDestino = maxSecuenciaDestino + 1;

            var cuotaIdDestino = $"{request.Cuota_ID}-{request.Especie_ID}-{request.ZonaDestino_ID}-{request.Año}-{nuevaSecuenciaDestino:D3}";

            var cuotaDestino = new Fac_02_Cuotas
            {
                Control_ID = cuotaIdDestino,
                Cuota_ID = request.Cuota_ID,
                Especie_ID = request.Especie_ID,
                Año = request.Año,
                Mes = request.Mes,
                Toneladas = request.Toneladas,
                Zona_ID = request.ZonaDestino_ID,
                Secuencia = nuevaSecuenciaDestino,
                Comentario = request.Comentario
            };

            _logger.LogInformation("Agregando cuota de origen con Control_ID: {Control_ID}", cuotaIdOrigen);
            await _fac02Repository.AddAsync(cuotaOrigen, usuarioSigo);

            _logger.LogInformation("Agregando cuota de destino con Control_ID: {Control_ID}", cuotaIdDestino);
            await _fac02Repository.AddAsync(cuotaDestino, usuarioSigo);

            _logger.LogInformation("Cambio de zona realizado exitosamente. Cuota de origen: {Control_ID_Origen}, Cuota de destino: {Control_ID_Destino}", cuotaIdOrigen, cuotaIdDestino);
            return Ok(new { codigoEstado = 1, mensaje = "Cambio de zona realizado exitosamente.", cuotaIdOrigen = cuotaOrigen.Control_ID, cuotaIdDestino = cuotaDestino.Control_ID });
        }
    }
}