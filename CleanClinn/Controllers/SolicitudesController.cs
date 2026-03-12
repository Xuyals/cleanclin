using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CleanClinn.Data;
using CleanClinn.DTOs;
using CleanClinn.Models;

namespace CleanClinn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SolicitudesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SolicitudesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET api/solicitudes — Usuario ve sus solicitudes / Trabajador ve las disponibles
        [HttpGet]
        public async Task<IActionResult> GetSolicitudes()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var rol = User.FindFirstValue(ClaimTypes.Role);

            List<Solicitud> solicitudes;

            if (rol == "Trabajador")
            {
                // Trabajador ve las pendientes (para aceptar) o las que son suyas
                solicitudes = await _context.Solicitudes
                    .Include(s => s.Usuario)
                    .Include(s => s.TipoServicio)
                    .Include(s => s.Trabajador).ThenInclude(t => t.Usuario)
                    .Where(s => s.Estado == EstadoSolicitud.Pendiente || s.TrabajadorId != null)
                    .OrderByDescending(s => s.FechaCreacion)
                    .ToListAsync();
            }
            else if (rol == "Admin")
            {
                solicitudes = await _context.Solicitudes
                    .Include(s => s.Usuario)
                    .Include(s => s.TipoServicio)
                    .Include(s => s.Trabajador).ThenInclude(t => t.Usuario)
                    .OrderByDescending(s => s.FechaCreacion)
                    .ToListAsync();
            }
            else
            {
                // Usuario normal ve solo sus solicitudes
                solicitudes = await _context.Solicitudes
                    .Include(s => s.TipoServicio)
                    .Include(s => s.Trabajador).ThenInclude(t => t.Usuario)
                    .Where(s => s.UsuarioId == userId)
                    .OrderByDescending(s => s.FechaCreacion)
                    .ToListAsync();
            }

            var resultado = solicitudes.Select(s => new SolicitudResponseDTO
            {
                Id = s.Id,
                TipoServicio = s.TipoServicio?.Nombre,
                Estado = s.Estado.ToString(),
                CostoEstimado = s.CostoEstimado,
                Direccion = s.Direccion,
                NombreUsuario = s.Usuario?.Nombre,
                NombreTrabajador = s.Trabajador?.Usuario?.Nombre,
                FechaCreacion = s.FechaCreacion.ToString("yyyy-MM-dd HH:mm")
            });

            return Ok(resultado);
        }

        // POST api/solicitudes — Usuario crea una solicitud
        [HttpPost]
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> CrearSolicitud([FromBody] CrearSolicitudDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var tipoServicio = await _context.TiposServicio.FindAsync(dto.TipoServicioId);
            if (tipoServicio == null)
                return BadRequest(new { mensaje = "Tipo de servicio no válido." });

            var solicitud = new Solicitud
            {
                UsuarioId = userId,
                TipoServicioId = dto.TipoServicioId,
                Direccion = dto.Direccion,
                Descripcion = dto.Descripcion,
                CostoEstimado = tipoServicio.PrecioBase,
                Estado = EstadoSolicitud.Pendiente
            };

            _context.Solicitudes.Add(solicitud);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Solicitud creada exitosamente.", id = solicitud.Id });
        }

        // PUT api/solicitudes/{id}/aceptar — Trabajador acepta una solicitud
        [HttpPut("{id}/aceptar")]
        [Authorize(Roles = "Trabajador")]
        public async Task<IActionResult> AceptarSolicitud(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var trabajador = await _context.Trabajadores
                .FirstOrDefaultAsync(t => t.UsuarioId == userId);

            if (trabajador == null)
                return NotFound(new { mensaje = "Perfil de trabajador no encontrado." });

            var solicitud = await _context.Solicitudes.FindAsync(id);

            if (solicitud == null)
                return NotFound(new { mensaje = "Solicitud no encontrada." });

            if (solicitud.Estado != EstadoSolicitud.Pendiente)
                return BadRequest(new { mensaje = "La solicitud ya no está disponible." });

            solicitud.TrabajadorId = trabajador.Id;
            solicitud.Estado = EstadoSolicitud.Aceptada;
            solicitud.FechaAceptacion = DateTime.Now;

            trabajador.Disponible = false;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Solicitud aceptada correctamente." });
        }

        // PUT api/solicitudes/{id}/completar — Trabajador marca como completada
        [HttpPut("{id}/completar")]
        [Authorize(Roles = "Trabajador")]
        public async Task<IActionResult> CompletarSolicitud(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var trabajador = await _context.Trabajadores.FirstOrDefaultAsync(t => t.UsuarioId == userId);

            var solicitud = await _context.Solicitudes.FindAsync(id);

            if (solicitud == null || solicitud.TrabajadorId != trabajador?.Id)
                return NotFound(new { mensaje = "Solicitud no encontrada o no asignada a este trabajador." });

            solicitud.Estado = EstadoSolicitud.Completada;
            solicitud.FechaCompletado = DateTime.Now;
            trabajador.Disponible = true;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Servicio marcado como completado." });
        }

        // PUT api/solicitudes/{id}/cancelar — Usuario cancela su solicitud
        [HttpPut("{id}/cancelar")]
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> CancelarSolicitud(int id, [FromBody] string motivo)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var solicitud = await _context.Solicitudes
                .FirstOrDefaultAsync(s => s.Id == id && s.UsuarioId == userId);

            if (solicitud == null)
                return NotFound(new { mensaje = "Solicitud no encontrada." });

            if (solicitud.Estado == EstadoSolicitud.Completada || solicitud.Estado == EstadoSolicitud.Cancelada)
                return BadRequest(new { mensaje = "No se puede cancelar esta solicitud." });

            solicitud.Estado = EstadoSolicitud.Cancelada;
            solicitud.MotivoCancelacion = motivo;

            // Liberar trabajador si ya había aceptado
            if (solicitud.TrabajadorId.HasValue)
            {
                var trabajador = await _context.Trabajadores.FindAsync(solicitud.TrabajadorId);
                if (trabajador != null)
                    trabajador.Disponible = true;
            }

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Solicitud cancelada." });
        }
    }
}