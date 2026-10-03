using System;
using System.Collections.Generic;
using System.Text;

namespace PollosTrack.Catalogos.Models
{
    public class Rol
    {
        public int RolId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
}
