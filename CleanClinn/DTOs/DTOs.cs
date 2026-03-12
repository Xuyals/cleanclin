using System.ComponentModel.DataAnnotations;

namespace CleanClinn.DTOs
{
    // ---- AUTH ----
    public class RegisterDTO
    {
        [Required] public string Nombre { get; set; }
        [Required][EmailAddress] public string Email { get; set; }
        [Required][MinLength(6)] public string Password { get; set; }
        [Required] public string Telefono { get; set; }
        public string Direccion { get; set; }
        // Si es trabajador, envía su especialidad
        public string Especialidad { get; set; }
    }

    public class LoginDTO
    {
        [Required][EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }
    }

    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public string Nombre { get; set; }
        public string Rol { get; set; }
        public int UsuarioId { get; set; }
    }

    // ---- SOLICITUD ----
    public class CrearSolicitudDTO
    {
        [Required] public int TipoServicioId { get; set; }
        [Required] public string Direccion { get; set; }
        public string Descripcion { get; set; }
    }

    public class SolicitudResponseDTO
    {
        public int Id { get; set; }
        public string TipoServicio { get; set; }
        public string Estado { get; set; }
        public decimal CostoEstimado { get; set; }
        public string Direccion { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreTrabajador { get; set; }
        public string FechaCreacion { get; set; }
    }

    // ---- CALIFICACION ----
    public class CalificacionDTO
    {
        [Required] public int SolicitudId { get; set; }
        [Required][Range(1, 5)] public int Puntaje { get; set; }
        public string Comentario { get; set; }
    }

    // ---- TRABAJADOR ----
    public class TrabajadorResponseDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Especialidad { get; set; }
        public double Calificacion { get; set; }
        public bool Disponible { get; set; }
        public string Descripcion { get; set; }
    }
}