namespace BackEnd.Models
{
    /// <summary>
    /// Representa una solicitud de operación de parte de pesca diario.
    /// </summary>
    public class OperacionRequest
    {
        /// <summary>
        /// ID del Centro de Costos.
        /// </summary>
        public string CcoId { get; set; }

        /// <summary>
        /// Fecha de la operación.
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// ID de la Marea.
        /// </summary>
        public string MareaId { get; set; }

        /// <summary>
        /// ID de la Operatividad.
        /// </summary>
        public string OperatividadId { get; set; }

        /// <summary>
        /// Millas náuticas recorridas.
        /// </summary>
        public decimal? MillasNauticas { get; set; }

        /// <summary>
        /// Fuerza del viento.
        /// </summary>
        public int? FuerzaViento { get; set; }

        /// <summary>
        /// Latitud de la operación.
        /// </summary>
        public double? Latitud { get; set; }

        /// <summary>
        /// Longitud de la operación.
        /// </summary>
        public double? Longitud { get; set; }

        /// <summary>
        /// Número de lances realizados.
        /// </summary>
        public int? Lances { get; set; }

        /// <summary>
        /// Consumo de petróleo.
        /// </summary>
        public decimal? ConsumoPetroleo { get; set; }

        /// <summary>
        /// ID de la Carnada.
        /// </summary>
        public string? CarnadaId { get; set; }

        /// <summary>
        /// Consumo de carnada.
        /// </summary>
        public decimal? ConsumoCarnada { get; set; }

        /// <summary>
        /// Número de anzuelos utilizados.
        /// </summary>
        public int? Anzuelos { get; set; }

        /// <summary>
        /// Horas de pesca.
        /// </summary>
        public decimal? HrsPesca { get; set; }

        /// <summary>
        /// Horas de ruta.
        /// </summary>
        public decimal? HrsRuta { get; set; }

        /// <summary>
        /// Horas de capa.
        /// </summary>
        public decimal? HrsCapa { get; set; }

        /// <summary>
        /// Horas de avería.
        /// </summary>
        public decimal? HrsAveria { get; set; }

        /// <summary>
        /// Horas de accidente.
        /// </summary>
        public decimal? HrsAccidente { get; set; }

        /// <summary>
        /// Horas de personal.
        /// </summary>
        public decimal? HrsPersonal { get; set; }
    }
}
