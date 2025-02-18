using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BackEnd.Models;
using BackEnd.Services;
using BackEnd.Interfaces;
using BackEnd.Filters;
using BackEnd.Constants;
using BackEnd.Data;

namespace BackEnd.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ValidateUserFilter))]
    public class Dim99UsuariosController : ControllerBase
    {
        private readonly IDim99Repository _dim99Repository;
        private readonly EmailService _emailService;
        private readonly ILogger<Dim99UsuariosController> _logger;

        public Dim99UsuariosController(IDim99Repository dim99Repository, EmailService emailService, ILogger<Dim99UsuariosController> logger)
        {
            _dim99Repository = dim99Repository;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Agrega nuevo Usuario.
        /// </summary>
        /// <param name="createRequest">Los detalles del Usuario a agregar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("nuevo")]
        public async Task<IActionResult> PostDim99Usuarios([FromBody] Dim99UsuariosCreateRequest createRequest, [FromQuery] string usuarioSigo)
        {
            if (string.IsNullOrEmpty(createRequest.Usuario_ID) || string.IsNullOrEmpty(createRequest.NombreUsuario) || string.IsNullOrEmpty(createRequest.CorreoElectronico) || string.IsNullOrEmpty(createRequest.TipoUsuario))
            {
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            _logger.LogInformation("Iniciando PostDim99Usuarios para Usuario_ID: {Usuario_ID}", createRequest.Usuario_ID);

            // Validación opcional: Verificar duplicados
            _logger.LogDebug("Verificando si el Usuario_ID : {Usuario_ID} ya existe", createRequest.Usuario_ID);
            if (await _dim99Repository.ExistsAsync(createRequest.Usuario_ID))
            {
                _logger.LogWarning("El Usuario_ID ya existe: {Usuario_ID}", createRequest.Usuario_ID);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.UsuarioDuplicado });
            }

            // Generar contraseña aleatoria
            _logger.LogDebug("Generando contraseña aleatoria para el nuevo usuario");
            var randomPassword = GenerateRandomPassword();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(randomPassword);
            var dim99Usuarios = new Dim_99_Usuarios
            {
                Usuario_ID = createRequest.Usuario_ID,
                NombreUsuario = createRequest.NombreUsuario,
                CorreoElectronico = createRequest.CorreoElectronico,
                TipoUsuario = createRequest.TipoUsuario,
                Cco_ID = createRequest.Cco_ID ?? "9999",
                Password = hashedPassword, // Almacenar la contraseña encriptada
                Reset = "SI", //"SI" para indicar que la contraseña es temporal y debe ser cambiada
                FechaCreacion = DateTime.Now,
                Estado = "A" // Estado activo por defecto
            };

            _logger.LogDebug("Fecha de creación del usuario: {FechaCreacion}", dim99Usuarios.FechaCreacion);

            // Inserción
            _logger.LogDebug("Insertando el nuevo usuario en la base de datos");
            await _dim99Repository.AddAsync(dim99Usuarios, usuarioSigo);

            // Enviar correo electrónico
            _logger.LogInformation("Enviando correo electrónico a {CorreoElectronico}", dim99Usuarios.CorreoElectronico);
            await _emailService.SendUserCreationEmailAsync(dim99Usuarios.CorreoElectronico, randomPassword, dim99Usuarios.Usuario_ID);
            _logger.LogInformation("Correo electrónico enviado a {CorreoElectronico}", dim99Usuarios.CorreoElectronico);

            _logger.LogInformation("Usuario creado exitosamente: {Usuario_ID}", dim99Usuarios.Usuario_ID);

            _logger.LogInformation("Fin de PostDim99Usuarios para Usuario_ID: {Usuario_ID}", createRequest.Usuario_ID);
            return Ok(new { codigoEstado = 1, mensaje = Messages.UsuarioCreado, usuarioId = dim99Usuarios.Usuario_ID });
        }

        /// <summary>
        /// Modifica Usuario existente.
        /// </summary>
        /// <param name="id">El ID del Usuario a modificar.</param>
        /// <param name="updateRequest">Los detalles de la actualización.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPut("{id}/modifica")]
        public async Task<IActionResult> PutDim99Usuarios(string id, [FromBody] Dim99UsuariosUpdateRequest updateRequest, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de PutDim99Usuarios para Usuario_ID: {Usuario_ID}", id);

            if (string.IsNullOrEmpty(updateRequest.NombreUsuario) || string.IsNullOrEmpty(updateRequest.CorreoElectronico) || string.IsNullOrEmpty(updateRequest.TipoUsuario))
            {
                _logger.LogWarning("Datos inválidos para Usuario_ID: {Usuario_ID}", id);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            var existingUsuario = await _dim99Repository.GetByIdAsync(id);
            if (existingUsuario == null)
            {
                _logger.LogWarning("Registro no encontrado para Usuario_ID: {Usuario_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            existingUsuario.NombreUsuario = updateRequest.NombreUsuario ?? existingUsuario.NombreUsuario;
            existingUsuario.CorreoElectronico = updateRequest.CorreoElectronico ?? existingUsuario.CorreoElectronico;
            existingUsuario.TipoUsuario = updateRequest.TipoUsuario ?? existingUsuario.TipoUsuario;

            _logger.LogInformation("Modificando Usuario para Usuario_ID: {Usuario_ID}", id);
            await _dim99Repository.UpdateAsync(existingUsuario, usuarioSigo);
            _logger.LogInformation("Usuario modificado exitosamente para Usuario_ID: {Usuario_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.UsuarioActualizado, usuarioId = id });
        }

        /// <summary>
        /// Cambia Estado de Usuario existente (Activo/Inactivo).
        /// </summary>
        /// <param name="id">El ID del Usuario a modificar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpDelete("{id}/cambia-estado")]
        public async Task<IActionResult> DeleteDim99Usuarios(string id, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de DeleteDim99Usuarios para Usuario_ID: {Usuario_ID}", id);

            var existingUsuario = await _dim99Repository.GetByIdAsync(id);
            if (existingUsuario == null)
            {
                _logger.LogWarning("Registro no encontrado para Usuario_ID: {Usuario_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            _logger.LogInformation("Cambiando estado de Usuario para Usuario_ID: {Usuario_ID}", id);
            existingUsuario.Estado = existingUsuario.Estado == "A" ? "I" : "A";
            await _dim99Repository.ChangeStateAsync(existingUsuario, usuarioSigo);
            _logger.LogInformation("Estado de Usuario cambiado exitosamente para Usuario_ID: {Usuario_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.CambioEstado, usuarioId = id, nuevoEstado = existingUsuario.Estado });
        }

        /// <summary>
        /// Restablece Contraseña de Usuario existente.
        /// </summary>
        /// <param name="id">El ID del Usuario a modificar.</param>
        /// <param name="usuarioSigo">El ID del usuario que realiza la operación.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(string id, [FromQuery] string usuarioSigo)
        {
            _logger.LogInformation("Inicio de ResetPassword para Usuario_ID: {Usuario_ID}", id);

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(usuarioSigo))
            {
                _logger.LogWarning("Datos inválidos para Usuario_ID: {Usuario_ID}", id);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.DatosInvalidos });
            }

            var existingUsuario = await _dim99Repository.GetByIdAsync(id);
            if (existingUsuario == null)
            {
                _logger.LogWarning("Registro no encontrado para Usuario_ID: {Usuario_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.RegistroNoExiste });
            }

            // Generar nueva contraseña aleatoria
            var randomPassword = GenerateRandomPassword();
            existingUsuario.Password = BCrypt.Net.BCrypt.HashPassword(randomPassword);
            existingUsuario.Reset = "SI";

            _logger.LogInformation("Restableciendo contraseña para Usuario_ID: {Usuario_ID}", id);
            await _dim99Repository.ResetPassAsync(existingUsuario, usuarioSigo);

            // Enviar correo electrónico con la nueva contraseña
            _logger.LogInformation("Enviando correo electrónico a {CorreoElectronico}", existingUsuario.CorreoElectronico);
            await _emailService.SendPasswordResetEmailAsync(existingUsuario.CorreoElectronico, randomPassword, existingUsuario.Usuario_ID);
            _logger.LogInformation("Correo electrónico enviado a {CorreoElectronico}", existingUsuario.CorreoElectronico);

            return Ok(new { codigoEstado = 1, mensaje = Messages.ResetPassword, usuarioId = id });
        }

        /// <summary>
        /// Permite al usuario cambiar su contraseña, ya sea obligatoriamente tras un reset o de manera voluntaria.
        /// </summary>
        /// <param name="id">El ID del usuario.</param>
        /// <param name="request">Objeto que contiene la contraseña actual, nueva y confirmación.</param>
        /// <returns>Retorna un mensaje de éxito o error en la operación.</returns>
        [HttpPost("{id}/cambia-password")]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePasswordRequest request)
        {
            _logger.LogInformation("Inicio de ChangePassword para Usuario_ID: {Usuario_ID}", id);

            if (string.IsNullOrEmpty(request.CurrentPassword) || string.IsNullOrEmpty(request.NewPassword) || string.IsNullOrEmpty(request.ConfirmNewPassword))
            {
                _logger.LogWarning("Datos inválidos para Usuario_ID: {Usuario_ID}", id);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.CamposObligados });
            }

            if (request.NewPassword != request.ConfirmNewPassword)
            {
                _logger.LogWarning("Las nuevas contraseñas no coinciden para Usuario_ID: {Usuario_ID}", id);
                return BadRequest(new { codigoEstado = 0, mensaje = Messages.PasswordDistintas });
            }

            var existingUsuario = await _dim99Repository.GetByIdAsync(id);
            if (existingUsuario == null)
            {
                _logger.LogWarning("Registro no encontrado para Usuario_ID: {Usuario_ID}", id);
                return NotFound(new { codigoEstado = 0, mensaje = Messages.UsuarioNoValido });
            }

            // Verificar la contraseña actual
            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, existingUsuario.Password))
            {
                _logger.LogWarning("Contraseña actual incorrecta para Usuario_ID: {Usuario_ID}", id);
                return Unauthorized(new { codigoEstado = 0, mensaje = Messages.PasswordActual });
            }

            existingUsuario.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            existingUsuario.Reset = "NO";

            _logger.LogInformation("Cambiando contraseña para Usuario_ID: {Usuario_ID}", id);
            await _dim99Repository.ChangePassAsync(existingUsuario, existingUsuario.Usuario_ID);
            _logger.LogInformation("Contraseña cambiada exitosamente para Usuario_ID: {Usuario_ID}", id);

            return Ok(new { codigoEstado = 1, mensaje = Messages.CambioPassword });
        }

        private string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            var random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}