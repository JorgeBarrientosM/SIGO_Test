namespace BackEnd.Helpers
{
    /// <summary>
    /// Clase estática que proporciona plantillas de correo electrónico.
    /// </summary>
    public static class EmailTemplates
    {
        private static readonly string Base64LogoSigo = Convert.ToBase64String(File.ReadAllBytes("Resources/ImagesSIGO/Logo-Color.svg"));
        private static readonly string Base64BannerJb = Convert.ToBase64String(File.ReadAllBytes("Resources/ImagesJB/Banner-Color.svg"));

        /// <summary>
        /// Obtiene la plantilla de correo electrónico para la creación de un nuevo usuario.
        /// </summary>
        /// <param name="temporaryPassword">La contraseña temporal del usuario.</param>
        /// <param name="user">El nombre de usuario.</param>
        /// <returns>La plantilla de correo electrónico en formato HTML.</returns>
        public static string GetUserCreationTemplate(string temporaryPassword, string user)
        {
            return $@"
                <html>
                <head>
                    <style>
                        body {{
                            font-family: 'Franklin Gothic Medium Cond', 'Franklin Gothic Medium', Arial, sans-serif;
                            font-size: 14px;
                            position: relative;
                            background: url('data:image/svg+xml;base64,{Base64LogoSigo}') no-repeat right top;
                            background-size: 500px 500px;
                            background-position: 420px top;
                        }}
                        .content {{
                            position: relative;
                            z-index: 1;
                            padding: 20px;
                            color: #404040;
                        }}
                    </style>
                </head>
                <body>
                    <div class='content'>
                        <h1 style='font-size: 34px; color: #358C9C; margin-bottom: 0;'>
                            <span style='color: #003296; font-size: 40px; font-weight: bold;'>S</span>istema 
                            <span style='color: #003296; font-size: 40px; font-weight: bold;'>I</span>ntegral de 
                            <span style='color: #003296; font-size: 40px; font-weight: bold;'>G</span>estión 
                            <span style='color: #003296; font-size: 40px; font-weight: bold;'>O</span>perativa
                        </h1>
                        <h2 style='color: #0097B2; font-size: 26px; margin-top: 0; font-weight: bold;'>
                            &quot;Creación Nuevo Usuario&quot;
                        </h2>
                        <p style='color: #009966; font-size: 22px; margin-top: 50px'><strong>Su cuenta ha sido creada exitosamente.</strong></p>
                        <p style='font-family: Arial, sans-serif; margin-bottom: 8px;'>Se le ha proporcionado una contraseña temporal que deberá cambiar al ingresar al sistema.</p>
                        <p style='padding-left: 30px; margin-top: 7px; font-family: Arial, sans-serif;'>
                            Credenciales para ingreso a sistema:<br>
                            <span style='display: block; padding-left: 20px; margin-top: 5px; margin-bottom: 5px;'>
                                • Usuario: {user}
                            </span>
                            <span style='display: inline; padding-left: 20px; font-family: Arial, sans-serif;'>
                                • Contraseña temporal: 
                                <strong>{temporaryPassword}</strong> 
                                <span style='color: gray; font-size: 11px; font-family: Arial, sans-serif;'>
                                    <i>(deberá cambiar contraseña al iniciar su primera sesión.)</i>
                                </span>
                            </span>
                            <span style='display: block; padding-left: 20px; margin-top: 5px; font-family: Arial, sans-serif;'>
                                • Enlace de conexión: <a href='https://SIGO.jbsoluciones.cl'>https://SIGO.jbsoluciones.cl</a>
                            </span>
                        </p>
                        <p style='margin-top: 50px; margin-bottom: 0; font-family: Arial, sans-serif;'>Atentamente,</p>
                        <p style='color: #334eac; font-size: 26px; margin-top: 5px; margin-bottom: 0;'><strong>Gestor de Usuarios SIGO</strong></p>
                        <div style='display: flex; align-items: center; margin-top: 3px; padding: 0;'>
                            <span style='color: #358C9C; font-size: 13px; font-family: Arial, sans-serif;'>Software desarrollado por &nbsp;</span>
                            <img src='data:image/svg+xml;base64,{Base64BannerJb}' alt='Banner JB Soluciones' width='360' height='66'>
                        </div>
                        <small style='font-family: Arial, sans-serif; color: gray; font-size: 10px;'><i>Correo electrónico generado de forma automática, favor no responder, ante cualquier duda o problema contáctenos a <a href='mailto:contacto@sigo.jbsoluciones.cl'>contacto@sigo.jbsoluciones.cl</a></i></small>
                    </div>
                </body>
                </html>";
        }

        /// <summary>
        /// Obtiene la plantilla de correo electrónico para el restablecimiento de contraseña.
        /// </summary>
        /// <param name="temporaryPassword">La contraseña temporal del usuario.</param>
        /// <param name="user">El nombre de usuario.</param>
        /// <returns>La plantilla de correo electrónico en formato HTML.</returns>
        public static string GetPasswordResetTemplate(string temporaryPassword, string user)
        {
            return $@"
                <html>
                <head>
                    <style>
                        body {{
                            font-family: 'Franklin Gothic Medium Cond', 'Franklin Gothic Medium', Arial, sans-serif;
                            font-size: 14px;
                            position: relative;
                            background: url('data:image/svg+xml;base64,{Base64LogoSigo}') no-repeat right top;
                            background-size: 500px 500px;
                            background-position: 420px top;
                        }}
                        .content {{
                            position: relative;
                            z-index: 1;
                            padding: 20px;
                            color: #404040;
                        }}
                    </style>
                </head>
                <body>
                    <div class='content'>
                        
                        <h1 style='font-size: 34px; color: #358C9C; margin-bottom: 0;'>
                            <span style='color: #003296; font-size: 40px; font-weight: bold;'>S</span>istema 
                            <span style='color: #003296; font-size: 40px; font-weight: bold;'>I</span>ntegral de 
                            <span style='color: #003296; font-size: 40px; font-weight: bold;'>G</span>estión 
                            <span style='color: #003296; font-size: 40px; font-weight: bold;'>O</span>perativa
                        </h1>
                        <h2 style='color: #0097B2; font-size: 26px; margin-top: 0; font-weight: bold;'>
                            &quot;Restablecimiento Contraseña&quot;
                        </h2>
                        <p style='color: #009966; font-size: 22px; margin-top: 50px'><strong>Su contraseña ha sido restablecida exitosamente.</strong></p>
                        <p style='font-family: Arial, sans-serif; margin-bottom: 8px;'>Se le ha proporcionado una contraseña temporal que deberá cambiar al reingresar al sistema.</p>
                        <p style='padding-left: 30px; margin-top: 7px; font-family: Arial, sans-serif;'>
                            Credenciales para ingreso a sistema:<br>
                            <span style='display: block; padding-left: 20px; margin-top: 5px; margin-bottom: 5px;'>
                                • Usuario: {user}
                            </span>
                            <span style='display: inline; padding-left: 20px; font-family: Arial, sans-serif;'>
                                • Contraseña temporal: 
                                <strong>{temporaryPassword}</strong> 
                                <span style='color: gray; font-size: 11px; font-family: Arial, sans-serif;'>
                                    <i>(deberá cambiar contraseña al iniciar una nueva sesión.)</i>
                                </span>
                            </span>
                            <span style='display: block; padding-left: 20px; margin-top: 5px; font-family: Arial, sans-serif;'>
                                • Enlace de conexión: <a href='https://SIGO.jbsoluciones.cl'>https://SIGO.jbsoluciones.cl</a>
                            </span>
                        </p>
                        <p style='margin-top: 50px; margin-bottom: 0; font-family: Arial, sans-serif;'>Atentamente,</p>
                        <p style='color: #334eac; font-size: 26px; margin-top: 5px; margin-bottom: 0;'><strong>Gestor de Usuarios SIGO</strong></p>
                        <div style='display: flex; align-items: center; margin-top: 3px; padding: 0;'>
                            <span style='color: #358C9C; font-size: 13px; font-family: Arial, sans-serif;'>Software desarrollado por &nbsp;</span>
                            <img src='data:image/svg+xml;base64,{Base64BannerJb}' alt='Banner JB Soluciones' width='360' height='66'>
                        </div>
                        <small style='font-family: Arial, sans-serif; color: gray; font-size: 10px;'><i>Correo electrónico generado de forma automática, favor no responder, ante cualquier duda o problema contáctenos a <a href='mailto:contacto@sigo.jbsoluciones.cl'>contacto@sigo.jbsoluciones.cl</a></i></small>
                    </div>
                </body>
                </html>";
        }
    }
}