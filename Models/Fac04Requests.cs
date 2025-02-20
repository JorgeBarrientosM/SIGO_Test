namespace BackEnd.Models
{
    /// <summary>
    /// Representa una línea de producción.
    /// </summary>
    public class ProduccionLinea
    {
        /// <summary>
        /// ID del Producto.
        /// </summary>
        public string ProductoId { get; set; }

        /// <summary>
        /// Número de lance.
        /// </summary>
        public int NroLance { get; set; }

        /// <summary>
        /// Cantidad de kilos.
        /// </summary>
        public decimal Kilos { get; set; }

        /// <summary>
        /// Cantidad de cajas.
        /// </summary>
        public int Cajas { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de producción.
    /// </summary>
    public class ProduccionRequest
    {
        /// <summary>
        /// ID de la Operación.
        /// </summary>
        public string OperacionId { get; set; }

        /// <summary>
        /// Fecha de la producción.
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// ID del Centro de Costos.
        /// </summary>
        public string CcoId { get; set; }

        /// <summary>
        /// ID de la Marea.
        /// </summary>
        public string MareaId { get; set; }

        /// <summary>
        /// Líneas de producción.
        /// </summary>
        public List<ProduccionLinea> Lineas { get; set; }
    }

    /// <summary>
    /// Representa una línea de certificación.
    /// </summary>
    public class CertificacionLinea
    {
        /// <summary>
        /// ID del Producto.
        /// </summary>
        public string ProductoId { get; set; }

        /// <summary>
        /// Cantidad de kilos.
        /// </summary>
        public decimal Kilos { get; set; }

        /// <summary>
        /// Cantidad de cajas.
        /// </summary>
        public int Cajas { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de certificación.
    /// </summary>
    public class CertificacionRequest
    {
        /// <summary>
        /// ID de la Marea.
        /// </summary>
        public string MareaId { get; set; }

        /// <summary>
        /// Total certificado.
        /// </summary>
        public decimal TotalCertificado { get; set; }

        /// <summary>
        /// Líneas de certificación.
        /// </summary>
        public List<CertificacionLinea> Lineas { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de descarga de producción.
    /// </summary>
    public class DescargaRequest
    {
        /// <summary>
        /// ID del Centro de Costos.
        /// </summary>
        public string CcoId { get; set; }

        /// <summary>
        /// ID de la Marea.
        /// </summary>
        public string MareaId { get; set; }

        /// <summary>
        /// Fecha de la certificación.
        /// </summary>
        public DateTime FechaCertificacion { get; set; }

        /// <summary>
        /// Número de certificación.
        /// </summary>
        public string NroCertificacion { get; set; }
    }

    /// <summary>
    /// Representa una solicitud de certificación de producción.
    /// </summary>
    public class CertificaRequest
    {
        /// <summary>
        /// ID del Centro de Costos.
        /// </summary>
        public string CcoId { get; set; }

        /// <summary>
        /// ID de la Marea.
        /// </summary>
        public string MareaId { get; set; }

        /// <summary>
        /// Fecha de la certificación.
        /// </summary>
        public DateTime FechaCertificacion { get; set; }

        /// <summary>
        /// Número de certificación.
        /// </summary>
        public string NroCertificacion { get; set; }

        /// <summary>
        /// ID del Producto.
        /// </summary>
        public string ProductoId { get; set; }

        /// <summary>
        /// Cantidad de kilos.
        /// </summary>
        public decimal Kilos { get; set; }

        /// <summary>
        /// Cantidad de cajas.
        /// </summary>
        public int Cajas { get; set; }
    }
}