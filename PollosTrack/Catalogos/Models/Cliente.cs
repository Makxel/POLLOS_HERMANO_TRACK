namespace PollosTrack.Catalogos.Models
{
    public class Cliente
    {
        public int ClienteId { get; set; }
        public string? Identificacion { get; set; }
        public string Nombre { get; set; } = "Consumidor Final";
        public string? Telefono { get; set; }
    }
}