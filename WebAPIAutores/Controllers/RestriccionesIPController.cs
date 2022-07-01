using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/restriccionesip")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RestriccionesIPController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;

        public RestriccionesIPController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Post(CrearRestriccionIPDTO crearRestriccionIPDTO)
        {
            var llaveDB = await _context.LlavesAPI.FirstOrDefaultAsync(x => x.Id == crearRestriccionIPDTO.LlaveId);

            if (llaveDB == null) { return NotFound(); }

            var usuarioId = ObtenerUsuariId();

            if (llaveDB.UsuarioId != usuarioId) { return Forbid(); }

            var restriccionIp = new RestriccionIP()
            {
                LlaveId = llaveDB.Id,
                IP = crearRestriccionIPDTO.IP
            };

            _context.Add(restriccionIp);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, ActualizarRestriccionIPDTO actualizarRestriccionIPDTO)
        {
            var restriccionDB = await _context.RestriccionesIP.Include(x => x.Llave)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (restriccionDB == null) { return NotFound(); }

            var usuarioId = ObtenerUsuariId();

            if (restriccionDB.Llave.UsuarioId != usuarioId) { return Forbid(); }

            restriccionDB.IP = actualizarRestriccionIPDTO.IP;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var restriccionDB = await _context.RestriccionesIP.Include(x => x.Llave).FirstOrDefaultAsync(x => x.Id==id);

            if (restriccionDB == null) { return NotFound(); }

            var usuarioId = ObtenerUsuariId();

            if (restriccionDB.Llave.UsuarioId !=usuarioId) { return Forbid(); }

            _context.Remove(restriccionDB);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
