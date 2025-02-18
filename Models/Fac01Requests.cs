namespace BackEnd.Models
{
    /// <summary>
    /// Representa una solicitud de creación de una nueva marea.
    /// </summary>
    public class CreacionMareaRequest
    {
        /// <summary>
        /// ID del Centro de Costos.
        /// </summary>
        public string CcoId { get; set; }

        /// <summary>
        /// ID del Objetivo.
        /// </summary>
        public string ObjetivoId { get; set; }

        /// <summary>
        /// ID de la Zona.
        /// </summary>
        public string ZonaId { get; set; }

        /// <summary>
        /// Número de la Marea.
        /// </summary>
        public int NumMarea { get; set; }

        /// <summary>
        /// Fecha de inicio de la Marea.
        /// </summary>
        public DateTime FechaInicio { get; set; }
    }

    /// <summary>
    /// Representa una solicitud para activar una marea.
    /// </summary>
    public class ActivarMareaRequest
    {
        /// <summary>
        /// ID de la Marea.
        /// </summary>
        public string MareaId { get; set; }
    }
}