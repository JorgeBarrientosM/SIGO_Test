using BackEnd.Helpers;
using System.Net;
using System.Net.Mail;

namespace BackEnd.Services
{
    /// <summary>
    /// Servicio para el envío de correos electrónicos.
    /// </summary>
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly SmtpClient _smtpClient;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EmailService"/>.
        /// </summary>
        /// <param name="configuration">La configuración de la aplicación.</param>
        /// <param name="logger">El logger para registrar información y errores.</param>
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, SmtpClient? smtpClient = null)
        {
            _configuration = configuration;
            _logger = logger;
            _smtpClient = smtpClient ?? CreateSmtpClient();
        }

        /// <summary>
        /// Crea una instancia de SmtpClient.
        /// </summary>
        /// <returns>Una instancia de SmtpClient.</returns>
        protected internal virtual SmtpClient CreateSmtpClient()
        {
            var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
            {
                Port = int.Parse(_configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
                EnableSsl = bool.Parse(_configuration["Smtp:EnableSsl"])
            };
            return smtpClient;
        }

        /// <summary>
        /// Envía un correo electrónico.
        /// </summary>
        /// <param name="toEmail">La dirección de correo electrónico del destinatario.</param>
        /// <param name="subject">El asunto del correo electrónico.</param>
        /// <param name="message">El mensaje del correo electrónico.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        /// <exception cref="ArgumentNullException">Lanzada cuando uno o más valores de configuración SMTP son nulos o vacíos.</exception>
        /// <exception cref="SmtpException">Lanzada cuando ocurre un error SMTP al enviar el correo electrónico.</exception>
        /// <exception cref="Exception">Lanzada cuando ocurre un error general al enviar el correo electrónico.</exception>
        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            if (string.IsNullOrEmpty(_configuration["Smtp:Host"]))
            {
                throw new ArgumentNullException("Smtp:Host");
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:Username"], _configuration["Smtp:SenderName"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            try
            {
                await _smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "Error SMTP al enviar el correo electrónico");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al enviar el correo electrónico");
                throw;
            }
        }

        /// <summary>
        /// Envía un correo electrónico de creación de usuario.
        /// </summary>
        /// <param name="toEmail">La dirección de correo electrónico del destinatario.</param>
        /// <param name="temporaryPassword">La contraseña temporal del usuario.</param>
        /// <param name="user">El nombre de usuario.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        public async Task SendUserCreationEmailAsync(string toEmail, string temporaryPassword, string user)
        {
            var subject = "SIGO: Creación Nuevo Usuario";
            var message = EmailTemplates.GetUserCreationTemplate(temporaryPassword, user);
            await SendEmailAsync(toEmail, subject, message);
        }

        /// <summary>
        /// Envía un correo electrónico de restablecimiento de contraseña.
        /// </summary>
        /// <param name="toEmail">La dirección de correo electrónico del destinatario.</param>
        /// <param name="temporaryPassword">La contraseña temporal del usuario.</param>
        /// <param name="user">El nombre de usuario.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        public async Task SendPasswordResetEmailAsync(string toEmail, string temporaryPassword, string user)
        {
            var subject = "SIGO: Restablecimiento Contraseña";
            var message = EmailTemplates.GetPasswordResetTemplate(temporaryPassword, user);
            await SendEmailAsync(toEmail, subject, message);
        }
    }
}