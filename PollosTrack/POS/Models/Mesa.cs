namespace PollosTrack.POS.Models
{
    public class Mesa
    {
        public int MesaId { get; set; }
        public int Numero { get; set; }
        public int Capacidad { get; set; }
        // La clave relaciona la mesa con CAT.ESTADOS; el código permite interpretar sus acciones.
        public int? EstadoId { get; set; }
        public string CodigoEstado { get; set; } = string.Empty;
    }
}
