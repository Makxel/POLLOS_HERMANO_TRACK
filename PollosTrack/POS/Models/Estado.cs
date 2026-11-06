namespace PollosTrack.POS.Models
{
    // Datos del catálogo compartido; los colores se administran en SQL Server.
    public class Estado
    {
        public int EstadoId { get; set; }
        public string Entidad { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string ColorHexadecimal { get; set; } = string.Empty;
    }
}
