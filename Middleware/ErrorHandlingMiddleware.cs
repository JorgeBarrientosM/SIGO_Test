using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BackEnd.Middleware
{
    /// <summary>
    /// Middleware para manejar errores globales en la aplicación.
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ErrorHandlingMiddleware"/>.
        /// </summary>
        /// <param name="next">El siguiente delegado de la solicitud en la canalización.</param>
        /// <param name="logger">El logger para registrar información y errores.</param>
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invoca el middleware para manejar la solicitud HTTP.
        /// </summary>
        /// <param name="context">El contexto HTTP.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió una excepción no controlada.");
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Maneja la excepción y envía una respuesta JSON con los detalles del error.
        /// </summary>
        /// <param name="context">El contexto HTTP.</param>
        /// <param name="exception">La excepción que ocurrió.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Ocurrió un error al procesar la solicitud.",
                Detailed = "Error interno del servidor"
                //Detailed = exception.Message // Puedes omitir esto en producción para no exponer detalles del error
            };

            var jsonResponse = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}