using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Data
{
    /// <summary>
    /// Contexto de la base de datos de la aplicación.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ApplicationDbContext"/>.
        /// </summary>
        /// <param name="options">Las opciones del contexto de la base de datos.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Tabla Dim_01_Cco.
        /// </summary>
        public DbSet<Dim_01_Cco> Dim_01_Cco { get; set; }

        /// <summary>
        /// Tabla Dim_02_ArtePesca.
        /// </summary>
        public DbSet<Dim_02_ArtePesca> Dim_02_ArtePesca { get; set; }

        /// <summary>
        /// Tabla Dim_03_Barcos.
        /// </summary>
        public DbSet<Dim_03_Barcos> Dim_03_Barcos { get; set; }

        /// <summary>
        /// Tabla Dim_04_Operatividad.
        /// </summary>
        public DbSet<Dim_04_Operatividad> Dim_04_Operatividad { get; set; }

        /// <summary>
        /// Tabla Dim_05_Zona.
        /// </summary>
        public DbSet<Dim_05_Zona> Dim_05_Zona { get; set; }

        /// <summary>
        /// Tabla Dim_06_Especies.
        /// </summary>
        public DbSet<Dim_06_Especies> Dim_06_Especies { get; set; }

        /// <summary>
        /// Tabla Dim_07_Corte.
        /// </summary>
        public DbSet<Dim_07_Corte> Dim_07_Corte { get; set; }

        /// <summary>
        /// Tabla Dim_08_Calibre.
        /// </summary>
        public DbSet<Dim_08_Calibre> Dim_08_Calibre { get; set; }

        /// <summary>
        /// Tabla Dim_09_Productos.
        /// </summary>
        public DbSet<Dim_09_Productos> Dim_09_Productos { get; set; }

        /// <summary>
        /// Tabla Dim_10_Precios.
        /// </summary>
        public DbSet<Dim_10_Precios> Dim_10_Precios { get; set; }

        /// <summary>
        /// Tabla Dim_11_Cuota.
        /// </summary>
        public DbSet<Dim_11_Cuota> Dim_11_Cuota { get; set; }

        /// <summary>
        /// Tabla Dim_12_Objetivo.
        /// </summary>
        public DbSet<Dim_12_Objetivo> Dim_12_Objetivo { get; set; }

        /// <summary>
        /// Tabla Dim_13_Carnada.
        /// </summary>
        public DbSet<Dim_13_Carnada> Dim_13_Carnada { get; set; }

        /// <summary>
        /// Tabla Dim_99_Usuarios.
        /// </summary>
        public DbSet<Dim_99_Usuarios> Dim_99_Usuarios { get; set; }

        /// <summary>
        /// Tabla Fac_01_Mareas.
        /// </summary>
        public DbSet<Fac_01_Mareas> Fac_01_Mareas { get; set; }

        /// <summary>
        /// Tabla Fac_02_Cuotas.
        /// </summary>
        public DbSet<Fac_02_Cuotas> Fac_02_Cuotas { get; set; }

        /// <summary>
        /// Tabla Fac_03_Operacion.
        /// </summary>
        public DbSet<Fac_03_Operacion> Fac_03_Operacion { get; set; }

        /// <summary>
        /// Tabla Fac_04_Produccion.
        /// </summary>
        public DbSet<Fac_04_Produccion> Fac_04_Produccion { get; set; }

        /// <summary>
        /// Tabla Fac_98_Log.
        /// </summary>
        public DbSet<Fac_98_Log> Fac_98_Log { get; set; }

        /// <summary>
        /// Tabla Fac_99_Sync.
        /// </summary>
        public DbSet<Fac_99_Sync> Fac_99_Sync { get; set; }
    }


    /// <summary>
    /// Clase para representar la tabla Dim_01_Cco.
    /// </summary>
    public class Dim_01_Cco
    {
        /// <summary>
        /// ID del Centro de Costos.
        /// </summary>
        [Key]
        public string Cco_ID { get; set; }

        /// <summary>
        /// Nombre del Centro de Costos.
        /// </summary>
        public string NombreCco { get; set; }

        /// <summary>
        /// Tipo del Centro de Costos.
        /// </summary>
        public string TipoCco { get; set; }

        /// <summary>
        /// Estado del Centro de Costos.
        /// </summary>
        public string Estado { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Dim_02_ArtePesca.
    /// </summary>
    public class Dim_02_ArtePesca
    {
        /// <summary>
        /// ID del Arte de Pesca.
        /// </summary>
        [Key]
        public string ArtePesca_ID { get; set; }

        /// <summary>
        /// Descripción del Arte de Pesca.
        /// </summary>
        public string DescripcionArtePesca { get; set; }

        /// <summary>
        /// Estado del Arte de Pesca.
        /// </summary>
        public string Estado { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Dim_03_Barcos.
    /// </summary>
    public class Dim_03_Barcos
    {
        /// <summary>
        /// ID del Centro de Costos.
        /// </summary>
        [Key]
        public string Cco_ID { get; set; }

        /// <summary>
        /// ID del Arte de Pesca.
        /// </summary>
        public string ArtePesca_ID { get; set; }

        /// <summary>
        /// Matrícula del Barco.
        /// </summary>
        public string Matricula { get; set; }

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

        /// <summary>
        /// Estado del Barco.
        /// </summary>
        public string Estado { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Dim_04_Operatividad.
    /// </summary>
    public class Dim_04_Operatividad
    {
        /// <summary>
        /// ID de la Operatividad.
        /// </summary>
        [Key]
        public string Operatividad_ID { get; set; }

        /// <summary>
        /// Detalle de la Operatividad.
        /// </summary>
        public string DetalleOperatividad { get; set; }

        /// <summary>
        /// Descripción de la Operatividad.
        /// </summary>
        public string DescripcionOperatividad { get; set; }

        /// <summary>
        /// Tipo de Operatividad.
        /// </summary>
        public string TipoOperatividad { get; set; }

        /// <summary>
        /// Estado de la Operatividad.
        /// </summary>
        public string Estado { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Dim_05_Zona.
    /// </summary>
    public class Dim_05_Zona
    {
        /// <summary>
        /// ID de la Zona.
        /// </summary>
        [Key]
        public string Zona_ID { get; set; }

        /// <summary>
        /// Descripción de la Zona.
        /// </summary>
        public string DescripcionZona { get; set; }

        /// <summary>
        /// Estado de la Zona.
        /// </summary>
        public string Estado { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Dim_06_Especies.
    /// </summary>
    public class Dim_06_Especies
    {
        /// <summary>
        /// ID de la Especie.
        /// </summary>
        [Key]
        public string Especie_ID { get; set; }

        /// <summary>
        /// Descripción de la Especie.
        /// </summary>
        public string DescripcionEspecie { get; set; }

        /// <summary>
        /// Grupo de la Especie.
        /// </summary>
        public string GrupoEspecie { get; set; }

        /// <summary>
        /// Estado de la Especie.
        /// </summary>
        public string Estado { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Dim_07_Corte.
    /// </summary>
    public class Dim_07_Corte
    {
        /// <summary>
        /// ID del Corte.
        /// </summary>
        [Key]
        public string Corte_ID { get; set; }

        /// <summary>
        /// Tipo de Corte.
        /// </summary>
        public string TipoCorte { get; set; }

        /// <summary>
        /// Descripción del Corte.
        /// </summary>
        public string DescripcionCorte { get; set; }

        /// <summary>
        /// Agregado del Corte.
        /// </summary>
        public string AgregadoCorte { get; set; }

        /// <summary>
        /// Estado del Corte.
        /// </summary>
        public string Estado { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Dim_08_Calibre.
    /// </summary>
    public class Dim_08_Calibre
    {
        /// <summary>
        /// ID del Calibre.
        /// </summary>
        [Key]
        public string Calibre_ID { get; set; }

        /// <summary>
        /// Descripción del Calibre.
        /// </summary>
        public string DescripcionCalibre { get; set; }

        /// <summary>
        /// Estado del Calibre.
        /// </summary>
        public string Estado { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Dim_09_Productos.
    /// </summary>
    public class Dim_09_Productos
    {
        /// <summary>
        /// ID del Producto.
        /// </summary>
        [Key]
        public string Producto_ID { get; set; }

        /// <summary>
        /// ID de la Especie.
        /// </summary>
        public string Especie_ID { get; set; }

        /// <summary>
        /// ID del Corte.
        /// </summary>
        public string Corte_ID { get; set; }

        /// <summary>
        /// ID del Calibre.
        /// </summary>
        public string Calibre_ID { get; set; }

        /// <summary>
        /// Descripción del Producto.
        /// </summary>
        public string DescripcionProducto { get; set; }

        /// <summary>
        /// Código JDE del Producto.
        /// </summary>
        public string CodigoJDE { get; set; }

        /// <summary>
        /// Factor del Producto.
        /// </summary>
        public decimal Factor { get; set; }

        /// <summary>
        /// Estado del Producto.
        /// </summary>
        public string Estado { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Dim_10_Precios.
    /// </summary>
    public class Dim_10_Precios
    {
        /// <summary>
        /// ID del Precio.
        /// </summary>
        [Key]
        public string Precio_ID { get; set; }

        /// <summary>
        /// ID del Arte de Pesca.
        /// </summary>
        public string ArtePesca_ID { get; set; }

        /// <summary>
        /// ID del Producto.
        /// </summary>
        public string Producto_ID { get; set; }

        /// <summary>
        /// ID de la Especie.
        /// </summary>
        public string Especie_ID { get; set; }

        /// <summary>
        /// ID del Corte.
        /// </summary>
        public string Corte_ID { get; set; }

        /// <summary>
        /// ID del Calibre.
        /// </summary>
        public string Calibre_ID { get; set; }

        /// <summary>
        /// Precio en USD.
        /// </summary>
        public decimal PrecioUSD { get; set; }

        /// <summary>
        /// Fecha Inicial del Precio.
        /// </summary>
        public DateTime FechaInicial { get; set; }

        /// <summary>
        /// Fecha Final del Precio.
        /// </summary>
        public DateTime? FechaFinal { get; set; }

        /// <summary>
        /// Estado del Precio.
        /// </summary>
        public string Estado { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Dim_11_Cuota.
    /// </summary>
    public class Dim_11_Cuota
    {
        /// <summary>
        /// ID del Movimiento de Cuota.
        /// </summary>
        [Key]
        public string Cuota_ID { get; set; }

        /// <summary>
        /// Descripción del Movimiento de Cuota.
        /// </summary>
        public string DescripcionMvtoCuota { get; set; }

        /// <summary>
        /// Tipo de Cuota.
        /// </summary>
        public string TipoCuota { get; set; }

        /// <summary>
        /// Tratamiento de la Cuota.
        /// </summary>
        public string Tratamiento { get; set; }

        /// <summary>
        /// Estado de la Cuota.
        /// </summary>
        public string Estado { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Dim_12_Objetivo.
    /// </summary>
    public class Dim_12_Objetivo
    {
        /// <summary>
        /// ID del Objetivo.
        /// </summary>
        [Key]
        public string Objetivo_ID { get; set; }

        /// <summary>
        /// Especies Objetivo.
        /// </summary>
        public string EspeciesObjetivo { get; set; }

        /// <summary>
        /// Estado del Objetivo.
        /// </summary>
        public string Estado { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Dim_13_Carnada.
    /// </summary>
    public class Dim_13_Carnada
    {
        /// <summary>
        /// ID de la Carnada.
        /// </summary>
        [Key]
        public string Carnada_ID { get; set; }

        /// <summary>
        /// Descripción de la Carnada.
        /// </summary>
        public string DescripcionCarnada { get; set; }

        /// <summary>
        /// Calibre de la Carnada.
        /// </summary>
        public string CalibreCarnada { get; set; }

        /// <summary>
        /// Piezas por Kilogramo de la Carnada.
        /// </summary>
        public int PiezasKg { get; set; }

        /// <summary>
        /// Estado de la Carnada.
        /// </summary>
        public string Estado { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Dim_99_Usuarios.
    /// </summary>
    public class Dim_99_Usuarios
    {
        /// <summary>
        /// ID del Usuario.
        /// </summary>
        [Key]
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
        /// Contraseña del Usuario.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Indicador de si la contraseña debe ser restablecida.
        /// </summary>
        public string Reset { get; set; }

        /// <summary>
        /// Fecha de creación del Usuario.
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Estado del Usuario.
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// ID del Centro de Costos.
        /// </summary>
        public string Cco_ID { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Fac_01_Mareas.
    /// </summary>
    public class Fac_01_Mareas
    {
        /// <summary>
        /// ID de la Marea.
        /// </summary>
        [Key]
        public string Marea_ID { get; set; }

        /// <summary>
        /// ID del Centro de Costos.
        /// </summary>
        public string Cco_ID { get; set; }

        /// <summary>
        /// Número de la Marea.
        /// </summary>
        public int NumMarea { get; set; }

        /// <summary>
        /// Fecha de inicio de la Marea.
        /// </summary>
        public DateTime FechaInicio { get; set; }

        /// <summary>
        /// Fecha de finalización de la Marea.
        /// </summary>
        public DateTime? FechaFinal { get; set; }

        /// <summary>
        /// ID del Objetivo.
        /// </summary>
        public string Objetivo_ID { get; set; }

        /// <summary>
        /// ID de la Zona.
        /// </summary>
        public string Zona_ID { get; set; }

        /// <summary>
        /// Estado operativo de la Marea.
        /// </summary>
        public string EstadoOperativo { get; set; }

        /// <summary>
        /// Estado de la Marea.
        /// </summary>
        public string Estado { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Fac_02_Cuotas.
    /// </summary>
    public class Fac_02_Cuotas
    {
        /// <summary>
        /// ID de la Cuota.
        /// </summary>
        [Key]
        public string Control_ID { get; set; }

        /// <summary>
        /// ID del Movimiento de Cuota.
        /// </summary>
        public string Cuota_ID { get; set; }

        /// <summary>
        /// ID de la Especie.
        /// </summary>
        public string Especie_ID { get; set; }

        /// <summary>
        /// Año de la Cuota.
        /// </summary>
        public int Año { get; set; }

        /// <summary>
        /// Mes de la Cuota.
        /// </summary>
        public int Mes { get; set; }

        /// <summary>
        /// Toneladas de la Cuota.
        /// </summary>
        public decimal Toneladas { get; set; }

        /// <summary>
        /// ID de la Zona.
        /// </summary>
        public string Zona_ID { get; set; }

        /// <summary>
        /// Secuencia de la Cuota.
        /// </summary>
        public int Secuencia { get; set; }

        /// <summary>
        /// Comentario adicional.
        /// </summary>
        public string Comentario { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Fac_03_Operacion.
    /// </summary>
    public class Fac_03_Operacion
    {
        /// <summary>
        /// ID de la Operación.
        /// </summary>
        [Key]
        public string Operacion_ID { get; set; }

        /// <summary>
        /// Versión de la Operación.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// ID del Centro de Costos.
        /// </summary>
        public string Cco_ID { get; set; }

        /// <summary>
        /// Fecha de la Operación.
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// ID de la Marea.
        /// </summary>
        public string Marea_ID { get; set; }

        /// <summary>
        /// ID de la Operatividad.
        /// </summary>
        public string Operatividad_ID { get; set; }

        /// <summary>
        /// Millas náuticas recorridas.
        /// </summary>
        public decimal MillasNauticas { get; set; }

        /// <summary>
        /// Fuerza del viento.
        /// </summary>
        public int FuerzaViento { get; set; }

        /// <summary>
        /// Latitud de la Operación.
        /// </summary>
        public double Latitud { get; set; }

        /// <summary>
        /// Longitud de la Operación.
        /// </summary>
        public double Longitud { get; set; }

        /// <summary>
        /// Número de lances realizados.
        /// </summary>
        public int Lances { get; set; }

        /// <summary>
        /// Consumo de petróleo.
        /// </summary>
        public decimal ConsumoPetroleo { get; set; }

        /// <summary>
        /// ID de la Carnada.
        /// </summary>
        public string? Carnada_ID { get; set; }

        /// <summary>
        /// Consumo de carnada.
        /// </summary>
        public decimal ConsumoCarnada { get; set; }

        /// <summary>
        /// Piezas de carnada utilizadas.
        /// </summary>
        public int PiezasCarnada { get; set; }

        /// <summary>
        /// Número de anzuelos utilizados.
        /// </summary>
        public int Anzuelos { get; set; }

        /// <summary>
        /// Horas de pesca.
        /// </summary>
        public decimal HrsPesca { get; set; }

        /// <summary>
        /// Horas de ruta.
        /// </summary>
        public decimal HrsRuta { get; set; }

        /// <summary>
        /// Horas de capa.
        /// </summary>
        public decimal HrsCapa { get; set; }

        /// <summary>
        /// Horas de avería.
        /// </summary>
        public decimal HrsAveria { get; set; }

        /// <summary>
        /// Horas de accidente.
        /// </summary>
        public decimal HrsAccidente { get; set; }

        /// <summary>
        /// Horas de personal.
        /// </summary>
        public decimal HrsPersonal { get; set; }

        /// <summary>
        /// Horas totales de la Operación.
        /// </summary>
        public decimal HrsTotal { get; set; }

        /// <summary>
        /// Indicador de sincronización.
        /// </summary>
        public string Sincronizado { get; set; }

        /// <summary>
        /// Indicador de estado de línea de operacion.
        /// </summary>
        public string Status { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Fac_04_Produccion.
    /// </summary>
    public class Fac_04_Produccion
    {
        /// <summary>
        /// ID de la Producción.
        /// </summary>
        [Key]
        public string Produccion_ID { get; set; }

        /// <summary>
        /// Versión de la Producción.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// ID de la Operación.
        /// </summary>
        public string Operacion_ID { get; set; }

        /// <summary>
        /// Fecha de la Producción.
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// ID del Centro de Costos.
        /// </summary>
        public string Cco_ID { get; set; }

        /// <summary>
        /// ID de la Marea.
        /// </summary>
        public string Marea_ID { get; set; }

        /// <summary>
        /// ID del Producto.
        /// </summary>
        public string Producto_ID { get; set; }

        /// <summary>
        /// Kilos producidos.
        /// </summary>
        public decimal Kilos { get; set; }

        /// <summary>
        /// Biomasa producida.
        /// </summary>
        public decimal Biomasa { get; set; }

        /// <summary>
        /// Precio en USD.
        /// </summary>
        public decimal PrecioUSD { get; set; }

        /// <summary>
        /// Total en USD.
        /// </summary>
        public decimal TotalUSD { get; set; }

        /// <summary>
        /// Precio de la biomasa.
        /// </summary>
        public decimal PrecioBio { get; set; }

        /// <summary>
        /// Indicador de sincronización.
        /// </summary>
        public string Sincronizado { get; set; }

        /// <summary>
        /// Número de lance.
        /// </summary>
        public int NroLance { get; set; }

        /// <summary>
        /// Certificación de la Producción.
        /// </summary>
        public string Certificacion { get; set; }

        /// <summary>
        /// Fecha de la Certificación.
        /// </summary>
        public DateTime? FechaCertificacion { get; set; }

        /// <summary>
        /// Número de la Certificación.
        /// </summary>
        public string? NroCertificacion { get; set; }

        /// <summary>
        /// Porcentaje de Producción.
        /// </summary>
        public decimal? PorcentajeProduccion { get; set; }

        /// <summary>
        /// Kilos certificados.
        /// </summary>
        public decimal? KgsCertificados { get; set; }

        /// <summary>
        /// Número de cajas.
        /// </summary>
        public int Cajas { get; set; }

        /// <summary>
        /// Peso medio de la Producción.
        /// </summary>
        public decimal? PesoMedio { get; set; }

        /// <summary>
        /// Indicador de estado de línea de operacion.
        /// </summary>
        public string Status { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Fac_98_Log.
    /// </summary>
    public class Fac_98_Log
    {
        /// <summary>
        /// ID del Log.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Log_ID { get; set; }

        /// <summary>
        /// Fecha y hora del Log.
        /// </summary>
        public DateTime FechaHora { get; set; }

        /// <summary>
        /// Módulo del Log.
        /// </summary>
        public string Modulo { get; set; }

        /// <summary>
        /// Evento del Log.
        /// </summary>
        public string Evento { get; set; }

        /// <summary>
        /// Detalle del Log.
        /// </summary>
        public string Detalle { get; set; }

        /// <summary>
        /// ID del Usuario que generó el Log.
        /// </summary>
        public string Usuario_ID { get; set; }
    }

    /// <summary>
    /// Clase para representar la tabla Fac_99_Sync.
    /// </summary>
    public class Fac_99_Sync
    {
        /// <summary>
        /// ID de la Sincronización.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sync_ID { get; set; }

        /// <summary>
        /// Orden de la Sincronización.
        /// </summary>
        public string Orden { get; set; }

        /// <summary>
        /// ID del Usuario que generó la Sincronización.
        /// </summary>
        public string Usuario_ID { get; set; }

        /// <summary>
        /// Fecha de la Sincronización.
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Hora de la Sincronización.
        /// </summary>
        public TimeSpan Hora { get; set; }

        /// <summary>
        /// Tipo de Evento de la Sincronización.
        /// </summary>
        public string TipoEvento { get; set; }

        /// <summary>
        /// Origen de la Sincronización.
        /// </summary>
        public string Origen { get; set; }

        /// <summary>
        /// Detalle de la Sincronización.
        /// </summary>
        public string Detalle { get; set; }
    }
}
