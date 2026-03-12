using System;
using System.ComponentModel.DataAnnotations;

namespace CleanClinn.Models
{
    public enum EstadoSolicitud
    {
        Pendiente,
        Aceptada,
        EnProceso,
        Completada,
        Cancelada
    }

    public class TipoServicio
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } // Limpieza, Plomería, Electricidad...

        [MaxLength(300)]
        public string Descripcion { get; set; }

        public decimal PrecioBase { get; set; }
    }

    public class Solicitud
    {
        public int Id { get; set; }

        // Quién solicita
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        // Qué tipo de servicio
        public int TipoServicioId { get; set; }
        public TipoServicio TipoServicio { get; set; }

        // Trabajador asignado (puede ser null hasta que acepte)
        public int? TrabajadorId { get; set; }
        public Trabajador Trabajador { get; set; }

        [Required]
        [MaxLength(300)]
        public string Direccion { get; set; }

        [MaxLength(500)]
        public string Descripcion { get; set; }

        public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;

        public decimal CostoEstimado { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaAceptacion { get; set; }

        public DateTime? FechaCompletado { get; set; }

        public string MotivoCancelacion { get; set; }
    }
}