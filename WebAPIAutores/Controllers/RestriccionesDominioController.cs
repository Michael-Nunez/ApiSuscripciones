using AutoMapper;
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
    [Route("api/restriccionesdominio")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RestriccionesDominioController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;

        public RestriccionesDominioController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Post(CrearRestriccionesDominioDTO crearRestriccionesDominioDTO)
        {
            var llaveDB = await _context.LlavesAPI.FirstOrDefaultAsync(x => x.Id == crearRestriccionesDominioDTO.LlaveId);

            if (llaveDB == null) { return NotFound(); }

            var usuarioId = ObtenerUsuariId();

            if (llaveDB.UsuarioId != usuarioId) { return Forbid(); }

            var restriccionDomino = new RestriccionDominio()
            {
                LlaveId = crearRestriccionesDominioDTO.LlaveId,
                Dominio = crearRestriccionesDominioDTO.Dominio
            };

            _context.Add(restriccionDomino);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, ActualizarRestriccionDominioDTO actualizarRestriccionDominioDTO)
        {
            var restriccionDB = await _context.RestriccionesDominios.Include(x => x.Llave)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (restriccionDB == null) { return NotFound(); }

            var usuarioId = ObtenerUsuariId();

            if (restriccionDB.Llave.UsuarioId != usuarioId) { return Forbid(); }

            restriccionDB.Dominio = actualizarRestriccionDominioDTO.Dominio;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var restriccionDB = await _context.RestriccionesDominios.Include(x => x.Llave)
                .FirstOrDefaultAsync(x => x.Id ==id);

            if (restriccionDB == null) { return NotFound(); }

            var usuarioId = ObtenerUsuariId();

            if (usuarioId != restriccionDB.Llave.UsuarioId) { return Forbid(); }

            _context.Remove(restriccionDB);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
