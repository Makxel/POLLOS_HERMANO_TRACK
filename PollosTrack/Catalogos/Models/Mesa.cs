namespace PollosTrack.Catalogos.Models
{
    public class Mesa
    {
        public int MesaId { get; set; }
        public int Numero { get; set; }
        public int Capacidad { get; set; } = 4;
        public int EstadoId { get; set; }
    }
}