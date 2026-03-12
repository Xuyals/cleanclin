using System;
using System.ComponentModel.DataAnnotations;

namespace CleanClinn.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(20)]
        public string Telefono { get; set; }

        [MaxLength(200)]
        public string Direccion { get; set; }

        // "Usuario" o "Trabajador" o "Admin"
        public string Rol { get; set; } = "Usuario";

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;
    }
}