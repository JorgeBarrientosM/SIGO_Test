using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackEnd.Data;
using BackEnd.Models;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Inicia sesión en la aplicación. Si el usuario tiene la contraseña reseteada, se requiere cambiarla antes de continuar.
        /// </summary>
        /// <param name="authRequest">Los detalles de autenticación del usuario.</param>
        /// <returns>Un token JWT si la autenticación es exitosa, o una indicación de que debe cambiar su contraseña.</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthRequest authRequest)
        {
            if (string.IsNullOrEmpty(authRequest.Username) || string.IsNullOrEmpty(authRequest.Password))
            {
                return BadRequest(new { mensaje = "Datos inválidos." });
            }

            var user = _context.Dim_99_Usuarios.SingleOrDefault(u => u.Usuario_ID == authRequest.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(authRequest.Password, user.Password))
            {
                return Unauthorized(new { mensaje = "Credenciales incorrectas." });
            }

            // Verificar si el usuario debe cambiar su contraseña
            if (user.Reset == "SI")
            {
                return Ok(new { codigoEstado = 2, mensaje = "Debe cambiar su contraseña antes de continuar.", requiereCambio = true });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Usuario_ID),
                    new Claim(ClaimTypes.Role, user.TipoUsuario)
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"])),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { codigoEstado = 1, mensaje = "Inicio de sesión exitoso.", token = tokenString });
        }

        /// <summary>
        /// Prueba el manejo de errores lanzando una excepción.
        /// </summary>
        /// <returns>Una excepción de prueba.</returns>
        [HttpGet("test-error")]
        public IActionResult TestError()
        {
            throw new Exception("Error de prueba.");
        }
    }
}