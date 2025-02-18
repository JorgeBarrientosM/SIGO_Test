using FluentValidation;
using BackEnd.Models;

namespace BackEnd.Validators
{
    /// <summary>
    /// Validador para la solicitud de autenticación.
    /// </summary>
    public class AuthRequestValidator : AbstractValidator<AuthRequest>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AuthRequestValidator"/>.
        /// </summary>
        public AuthRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("El nombre de usuario es obligatorio.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("La contraseña es obligatoria.");
        }
    }
}