using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CleanClinn.Data;
using CleanClinn.DTOs;

namespace CleanClinn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrabajadoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TrabajadoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET api/trabajadores — Todos los trabajadores disponibles (público)
        [HttpGet]
        public async Task<IActionResult> GetTrabajadores([FromQuery] string especialidad = null)
        {
            var query = _context.Trabajadores
                .Include(t => t.Usuario)
                .Where(t => t.Usuario.Activo);

            if (!string.IsNullOrEmpty(especialidad))
                query = query.Where(t => t.Especialidad.Contains(especialidad));

            var trabajadores = await query
                .OrderByDescending(t => t.Calificacion)
                .Select(t => new TrabajadorResponseDTO
                {
                    Id = t.Id,
                    Nombre = t.Usuario.Nombre,
                    Especialidad = t.Especialidad,
                    Calificacion = t.Calificacion,
                    Disponible = t.Disponible,
                    Descripcion = t.Descripcion
                })
                .ToListAsync();

            return Ok(trabajadores);
        }

        // GET api/trabajadores/{id} — Detalle de un trabajador
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrabajador(int id)
        {
            var t = await _context.Trabajadores
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (t == null)
                return NotFound(new { mensaje = "Trabajador no encontrado." });

            return Ok(new TrabajadorResponseDTO
            {
                Id = t.Id,
                Nombre = t.Usuario.Nombre,
                Especialidad = t.Especialidad,
                Calificacion = t.Calificacion,
                Disponible = t.Disponible,
                Descripcion = t.Descripcion
            });
        }
    }
}