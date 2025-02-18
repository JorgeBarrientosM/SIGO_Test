namespace BackEnd.Helpers
{
    public class IdDescriptions
    {
        public static readonly Dictionary<string, string> Descriptions = new()
    {
        { "Dim_01_Cco", "NombreCco" },
        { "Dim_02_ArtePesca", "DescripcionArtePesca" },
        { "Dim_03_Barcos", "Matricula" }, // Por ejemplo, usamos Matrícula como descripción
        { "Dim_04_Operatividad", "DescripcionOperatividad" },
        { "Dim_05_Zona", "DescripcionZona" },
        { "Dim_06_Especies", "DescripcionEspecie" },
        { "Dim_07_Corte", "DescripcionCorte" },
        { "Dim_08_Calibre", "DescripcionCalibre" },
        { "Dim_09_Productos", "DescripcionProducto" },
        { "Dim_10_Precios", "Producto_ID" }, // Podría ser otro campo si hay una descripción mejor
        { "Dim_11_Cuota", "DescripcionMvtoCuota" },
        { "Dim_12_Objetivo", "EspeciesObjetivo" },
        { "Dim_13_Carnada", "DescripcionCarnada" }//,
        //{ "Fac_01_Mareas", "EstadoOperativo" },
        //{ "Fac_02_Cuotas", "Comentario" }
    };
    }
}
