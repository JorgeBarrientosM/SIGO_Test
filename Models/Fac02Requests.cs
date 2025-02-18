namespace BackEnd.Models
{
    /// <summary>
    /// Representa una solicitud para aumentar cuotas.
    /// </summary>
    public class AumentaCuotasRequest
    {
        /// <summary>
        /// ID del movimiento de cuota.
        /// </summary>
        public string Cuota_ID { get; set; }

        /// <summary>
        /// ID de la especie.
        /// </summary>
        public string Especie_ID { get; set; }

        /// <summary>
        /// Año de la cuota.
        /// </summary>
        public int Año { get; set; }

        /// <summary>
        /// Mes de la cuota.
        /// </summary>
        public int Mes { get; set; }

        /// <summary>
        /// Toneladas de la cuota.
        /// </summary>
        public decimal Toneladas { get; set; }

        /// <summary>
        /// ID de la zona.
        /// </summary>
        public string Zona_ID { get; set; }

        /// <summary>
        /// Comentario adicional.
        /// </summary>
        public string Comentario { get; set; }
    }

    /// <summary>
    /// Representa una solicitud para rebajar cuotas.
    /// </summary>
    public class RebajaCuotasRequest
    {
        /// <summary>
        /// ID del movimiento de cuota.
        /// </summary>
        public string Cuota_ID { get; set; }

        /// <summary>
        /// ID de la especie.
        /// </summary>
        public string Especie_ID { get; set; }

        /// <summary>
        /// Año de la cuota.
        /// </summary>
        public int Año { get; set; }

        /// <summary>
        /// Mes de la cuota.
        /// </summary>
        public int Mes { get; set; }

        /// <summary>
        /// Toneladas de la cuota.
        /// </summary>
        public decimal Toneladas { get; set; }

        /// <summary>
        /// ID de la zona.
        /// </summary>
        public string Zona_ID { get; set; }

        /// <summary>
        /// Comentario adicional.
        /// </summary>
        public string Comentario { get; set; }
    }

    /// <summary>
    /// Representa una solicitud para cambiar la zona de pesca.
    /// </summary>
    public class CambioZonaRequest
    {
        /// <summary>
        /// ID del movimiento de cuota.
        /// </summary>
        public string Cuota_ID { get; set; }

        /// <summary>
        /// ID de la especie.
        /// </summary>
        public string Especie_ID { get; set; }

        /// <summary>
        /// Año de la cuota.
        /// </summary>
        public int Año { get; set; }

        /// <summary>
        /// Mes de la cuota.
        /// </summary>
        public int Mes { get; set; }

        /// <summary>
        /// Toneladas de la cuota.
        /// </summary>
        public decimal Toneladas { get; set; }

        /// <summary>
        /// ID de la zona de origen.
        /// </summary>
        public string ZonaOrigen_ID { get; set; }

        /// <summary>
        /// ID de la zona de destino.
        /// </summary>
        public string ZonaDestino_ID { get; set; }

        /// <summary>
        /// Comentario adicional.
        /// </summary>
        public string Comentario { get; set; }
    }
}