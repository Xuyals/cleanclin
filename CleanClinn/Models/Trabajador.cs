using System;
using System.ComponentModel.DataAnnotations;

namespace CleanClinn.Models
{
    public class Trabajador
    {
        public int Id { get; set; }

        // Relación con Usuario base
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [Required]
        [MaxLength(100)]
        public string Especialidad { get; set; } // limpieza, plomería, electricidad, etc.

        public double Calificacion { get; set; } = 0;

        public int TotalCalificaciones { get; set; } = 0;

        public bool Disponible { get; set; } = true;

        [MaxLength(500)]
        public string Descripcion { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}