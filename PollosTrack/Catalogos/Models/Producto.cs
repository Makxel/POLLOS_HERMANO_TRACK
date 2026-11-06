namespace PollosTrack.Catalogos.Models
{
    public class Producto
    {
        public int ProductoId { get; set; }
        public int CategoriaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public bool AplicaIva { get; set; } = true;
        public bool Activo { get; set; } = true;
    }
}