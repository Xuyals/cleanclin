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
    public class CalificacionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CalificacionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST api/calificaciones — Usuario califica al trabajador tras completar el servicio
        [HttpPost]
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> Calificar([FromBody] CalificacionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var solicitud = await _context.Solicitudes
                .Include(s => s.Trabajador)
                .FirstOrDefaultAsync(s => s.Id == dto.SolicitudId && s.UsuarioId == userId);

            if (solicitud == null)
                return NotFound(new { mensaje = "Solicitud no encontrada." });

            if (solicitud.Estado != EstadoSolicitud.Completada)
                return BadRequest(new { mensaje = "Solo se puede calificar un servicio completado." });

            // Verificar que no haya calificado ya
            bool yaCalificado = await _context.Calificaciones
                .AnyAsync(c => c.SolicitudId == dto.SolicitudId && c.UsuarioId == userId);

            if (yaCalificado)
                return BadRequest(new { mensaje = "Ya calificaste este servicio." });

            var calificacion = new Calificacion
            {
                SolicitudId = dto.SolicitudId,
                UsuarioId = userId,
                TrabajadorId = solicitud.TrabajadorId.Value,
                Puntaje = dto.Puntaje,
                Comentario = dto.Comentario
            };

            _context.Calificaciones.Add(calificacion);

            // Actualizar promedio del trabajador
            var trabajador = solicitud.Trabajador;
            trabajador.TotalCalificaciones++;
            trabajador.Calificacion = ((trabajador.Calificacion * (trabajador.TotalCalificaciones - 1)) + dto.Puntaje)
                                       / trabajador.TotalCalificaciones;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Calificación registrada exitosamente." });
        }

        // GET api/calificaciones/trabajador/{id}
        [HttpGet("trabajador/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCalificacionesTrabajador(int id)
        {
            var calificaciones = await _context.Calificaciones
                .Include(c => c.Usuario)
                .Where(c => c.TrabajadorId == id)
                .OrderByDescending(c => c.Fecha)
                .Select(c => new
                {
                    c.Puntaje,
                    c.Comentario,
                    Usuario = c.Usuario.Nombre,
                    Fecha = c.Fecha.ToString("yyyy-MM-dd")
                })
                .ToListAsync();

            return Ok(calificaciones);
        }
    }
}