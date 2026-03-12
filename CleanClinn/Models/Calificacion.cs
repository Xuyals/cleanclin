using System;
using System.ComponentModel.DataAnnotations;

namespace CleanClinn.Models
{
    public class Calificacion
    {
        public int Id { get; set; }

        public int SolicitudId { get; set; }
        public Solicitud Solicitud { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int TrabajadorId { get; set; }
        public Trabajador Trabajador { get; set; }

        [Range(1, 5)]
        public int Puntaje { get; set; }

        [MaxLength(500)]
        public string Comentario { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
