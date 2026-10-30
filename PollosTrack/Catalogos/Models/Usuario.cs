using System;

namespace PollosTrack.Catalogos.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public int RolId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}