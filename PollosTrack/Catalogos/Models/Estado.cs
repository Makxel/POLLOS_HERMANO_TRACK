namespace PollosTrack.Catalogos.Models
{
    public class Estado
    {
        public int EstadoId { get; set; }
        public string Entidad { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string ColorHex { get; set; } = string.Empty;
        public int Orden { get; set; }
    }
}