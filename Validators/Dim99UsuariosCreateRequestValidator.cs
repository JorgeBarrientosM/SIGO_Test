using FluentValidation;
using BackEnd.Models;

namespace BackEnd.Validators
{
    /// <summary>
    /// Validador para la solicitud de creación de un usuario.
    /// </summary>
    public class Dim99UsuariosCreateRequestValidator : AbstractValidator<Dim99UsuariosCreateRequest>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Dim99UsuariosCreateRequestValidator"/>.
        /// </summary>
        public Dim99UsuariosCreateRequestValidator()
        {
            RuleFor(x => x.Usuario_ID).NotEmpty().WithMessage("El ID de usuario es obligatorio.");
            RuleFor(x => x.NombreUsuario).NotEmpty().WithMessage("El nombre de usuario es obligatorio.");
            RuleFor(x => x.CorreoElectronico)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.");
            RuleFor(x => x.TipoUsuario).NotEmpty().WithMessage("El tipo de usuario es obligatorio.");
        }
    }
}