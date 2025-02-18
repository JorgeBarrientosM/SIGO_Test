using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using BackEnd.Interfaces;
using BackEnd.Constants;

namespace BackEnd.Filters
{
    /// <summary>
    /// Filtro para validar usuarios antes de ejecutar una acción.
    /// </summary>
    public class ValidateUserFilter : IAsyncActionFilter
    {
        private readonly IUserValidationService _userValidationService;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ValidateUserFilter"/>.
        /// </summary>
        /// <param name="userValidationService">El servicio de validación de usuarios.</param>
        public ValidateUserFilter(IUserValidationService userValidationService)
        {
            _userValidationService = userValidationService;
        }

        /// <summary>
        /// Método que se ejecuta antes de la acción para validar el usuario.
        /// </summary>
        /// <param name="context">El contexto de la acción.</param>
        /// <param name="next">El delegado para ejecutar la siguiente acción.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Obtener el parámetro "usuarioSigo" del método del controlador
            if (context.ActionArguments.TryGetValue("usuarioSigo", out var usuarioSigoObj) && usuarioSigoObj is string usuarioSigo)
            {
                // Validar el usuario usando el servicio
                var usuarioValido = await _userValidationService.ValidateUserAsync(usuarioSigo);

                if (!usuarioValido)
                {
                    context.Result = new BadRequestObjectResult(new
                    {
                        codigoEstado = 0,
                        mensaje = Messages.UsuarioNoValido
                    });
                    return;
                }
            }

            await next();
        }
    }
}