namespace BackEnd.Models
{
    /// <summary>
    /// Representa una solicitud de actualización para un Centro de Costos.
    /// </summary>
    public class Dim01CcoUpdateRequest
    {
        /// <summary>
        /// Nombre del Centro de Costos.
        /// </summary>
        public string? NombreCco { get; set; }

        /// <summary>
        /// Tipo del Centro de Costos.
        /// </summary>
        public string? TipoCco { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de actualización para un Arte de Pesca.
    /// </summary>
    public class Dim02ArtePescaUpdateRequest
    {
        /// <summary>
        /// Descripción del Arte de Pesca.
        /// </summary>
        public string? DescripcionArtePesca { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de actualización para un Barco.
    /// </summary>
    public class Dim03BarcosUpdateRequest
    {
        /// <summary>
        /// ID del Arte de Pesca.
        /// </summary>
        public string? ArtePesca_ID { get; set; }

        /// <summary>
        /// Matrícula del Barco.
        /// </summary>
        public string? Matricula { get; set; }

        /// <summary>
        /// Año del Barco.
        /// </summary>
        public int Año { get; set; }

        /// <summary>
        /// Tonelaje de Registro Bruto (TRG) del Barco.
        /// </summary>
        public decimal TRG { get; set; }

        /// <summary>
        /// Eslora del Barco.
        /// </summary>
        public decimal Eslora { get; set; }

        /// <summary>
        /// Manga del Barco.
        /// </summary>
        public decimal Manga { get; set; }

        /// <summary>
        /// Capacidad de Producción del Barco.
        /// </summary>
        public decimal CapProduccion { get; set; }

        /// <summary>
        /// Capacidad Máxima de Petróleo del Barco.
        /// </summary>
        public decimal CapMaxPetroleo { get; set; }

        /// <summary>
        /// Velocidad Máxima del Barco.
        /// </summary>
        public int VelocidadMaxima { get; set; }

        /// <summary>
        /// Número de Tripulantes del Barco.
        /// </summary>
        public int Tripulacion { get; set; }

        /// <summary>
        /// Consumo Máximo del Barco.
        /// </summary>
        public decimal ConsumoMaximo { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de actualización para una Operatividad.
    /// </summary>
    public class Dim04OperatividadUpdateRequest
    {
        /// <summary>
        /// Detalle de la Operatividad.
        /// </summary>
        public string? DetalleOperatividad { get; set; }

        /// <summary>
        /// Descripción de la Operatividad.
        /// </summary>
        public string? DescripcionOperatividad { get; set; }

        /// <summary>
        /// Tipo de Operatividad.
        /// </summary>
        public string? TipoOperatividad { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de actualización para una Zona.
    /// </summary>
    public class Dim05ZonaUpdateRequest
    {
        /// <summary>
        /// Descripción de la Zona.
        /// </summary>
        public string? DescripcionZona { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de actualización para una Especie.
    /// </summary>
    public class Dim06EspeciesUpdateRequest
    {
        /// <summary>
        /// Descripción de la Especie.
        /// </summary>
        public string? DescripcionEspecie { get; set; }
        public string? GrupoEspecie { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de actualización para un Corte.
    /// </summary>
    public class Dim07CorteUpdateRequest
    {
        /// <summary>
        /// Descripción del Corte.
        /// </summary>
        public string? DescripcionCorte { get; set; }
        public string? TipoCorte { get; set; }
        public string? AgregadoCorte { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de actualización para un Calibre.
    /// </summary>
    public class Dim08CalibreUpdateRequest
    {
        /// <summary>
        /// Descripción del Calibre.
        /// </summary>
        public string? DescripcionCalibre { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de actualización para un Producto.
    /// </summary>
    public class Dim09ProductosUpdateRequest
    {
        /// <summary>
        /// Código JDE del Producto.
        /// </summary>
        public string? CodigoJDE { get; set; }

        /// <summary>
        /// Factor del Producto.
        /// </summary>
        public decimal Factor { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de actualización para un Precio.
    /// </summary>
    public class Dim10PreciosUpdateRequest
    {
        /// <summary>
        /// Precio en USD.
        /// </summary>
        public decimal PrecioUSD { get; set; }

        /// <summary>
        /// Fecha Inicial del Precio.
        /// </summary>
        public DateTime? FechaInicial { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de actualización para una Cuota.
    /// </summary>
    public class Dim11CuotaUpdateRequest
    {
        /// <summary>
        /// Descripción del Movimiento de Cuota.
        /// </summary>
        public string? DescripcionMvtoCuota { get; set; }

        /// <summary>
        /// Tipo de Cuota.
        /// </summary>
        public string? TipoCuota { get; set; }

        /// <summary>
        /// Tratamiento de la Cuota.
        /// </summary>
        public string? Tratamiento { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de actualización para un Objetivo.
    /// </summary>
    public class Dim12ObjetivoUpdateRequest
    {
        /// <summary>
        /// Especies Objetivo.
        /// </summary>
        public string? EspeciesObjetivo { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de actualización para una Carnada.
    /// </summary>
    public class Dim13CarnadaUpdateRequest
    {
        /// <summary>
        /// Descripción de la Carnada.
        /// </summary>
        public string? DescripcionCarnada { get; set; }

        /// <summary>
        /// Calibre de la Carnada.
        /// </summary>
        public string? CalibreCarnada { get; set; }

        /// <summary>
        /// Piezas por Kilogramo de la Carnada.
        /// </summary>
        public int PiezasKg { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de creación de un Usuario.
    /// </summary>
    public class Dim99UsuariosCreateRequest
    {
        /// <summary>
        /// ID del Usuario.
        /// </summary>
        public string Usuario_ID { get; set; }

        /// <summary>
        /// Nombre del Usuario.
        /// </summary>
        public string NombreUsuario { get; set; }

        /// <summary>
        /// Correo Electrónico del Usuario.
        /// </summary>
        public string CorreoElectronico { get; set; }

        /// <summary>
        /// Tipo de Usuario.
        /// </summary>
        public string TipoUsuario { get; set; }

        /// <summary>
        /// ID del Centro de Costos.
        /// </summary>
        public string? Cco_ID { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de actualización para un Usuario.
    /// </summary>
    public class Dim99UsuariosUpdateRequest
    {
        /// <summary>
        /// Nombre del Usuario.
        /// </summary>
        public string? NombreUsuario { get; set; }

        /// <summary>
        /// Correo Electrónico del Usuario.
        /// </summary>
        public string? CorreoElectronico { get; set; }

        /// <summary>
        /// Tipo de Usuario.
        /// </summary>
        public string? TipoUsuario { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de cambio de contraseña.
    /// </summary>
    public class ChangePasswordRequest
    {
        /// <summary>
        /// Contraseña actual del usuario.
        /// </summary>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// Nueva contraseña a establecer.
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Confirmación de la nueva contraseña.
        /// </summary>
        public string ConfirmNewPassword { get; set; }
    }

}