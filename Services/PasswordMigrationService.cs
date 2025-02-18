using Microsoft.EntityFrameworkCore;
using BackEnd.Data;

/// <summary>
/// Servicio para migrar contraseñas de usuarios a un formato encriptado con BCrypt.
/// </summary>
public class PasswordMigrationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PasswordMigrationService> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="PasswordMigrationService"/>.
    /// </summary>
    /// <param name="context">El contexto de la base de datos.</param>
    /// <param name="logger">El logger para registrar información y errores.</param>
    public PasswordMigrationService(ApplicationDbContext context, ILogger<PasswordMigrationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Migra las contraseñas de los usuarios a un formato encriptado con BCrypt.
    /// </summary>
    /// <returns>Una tarea que representa la operación asincrónica.</returns>
    public async Task MigratePasswordsAsync()
    {
        var usuarios = await _context.Dim_99_Usuarios.ToListAsync();
        foreach (var usuario in usuarios)
        {
            // Verificar si la contraseña ya está encriptada con BCrypt
            if (!usuario.Password.StartsWith("$2a$") && !usuario.Password.StartsWith("$2b$") && !usuario.Password.StartsWith("$2y$"))
            {
                // Encriptar la contraseña con BCrypt
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
                _logger.LogInformation($"Contraseña de usuario {usuario.Usuario_ID} migrada.");
            }
        }

        // Guardar los cambios en la base de datos
        await _context.SaveChangesAsync();
        _logger.LogInformation("Migración de contraseñas completada.");
    }
}