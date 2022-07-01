using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;
using WebAPIAutores.Servicios;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/llavesapi")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LlavesAPIController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ServicioLlaves _servicioLlaves;

        public LlavesAPIController(ApplicationDbContext context, IMapper mapper, ServicioLlaves servicioLlaves)
        {
            _context = context;
            _mapper = mapper;
            _servicioLlaves = servicioLlaves;
        }

        [HttpGet]
        public async Task<List<LlaveDTO>> MisLlaves()
        {
            var usuarioId = ObtenerUsuariId();
            var llaves = await _context.LlavesAPI
                .Include(x => x.RestriccionesDominio)
                .Include(x => x.RestriccionesIP)
                .Where(x => x.UsuarioId == usuarioId).ToListAsync();
            return _mapper.Map<List<LlaveDTO>>(llaves);
        }

        [HttpPost]
        public async Task<ActionResult> CrearLlave(CrearLlaveDTO crearLlaveDTO)
        {
            var usuarioId = ObtenerUsuariId();

            if (crearLlaveDTO.TipoLlave == TipoLlave.Gratuita)
            {
                var elUsuarioYaTieneUnaLlaveGratuita = await _context.LlavesAPI.
                    AnyAsync(x => x.UsuarioId == usuarioId && x.TipoLlave == TipoLlave.Gratuita);

                if (elUsuarioYaTieneUnaLlaveGratuita) { return BadRequest("El usuario ya tiene una llave gratuita."); }
            }

            await _servicioLlaves.CrearLlave(usuarioId, crearLlaveDTO.TipoLlave);
            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult> ActualizarLlave(ActualizarLlaveDTO actualizarLlaveDTO)
        {
            var usuarioId = ObtenerUsuariId();

            var llaveDB = await _context.LlavesAPI.FirstOrDefaultAsync(x => x.UsuarioId == usuarioId);

            if (llaveDB == null) { return NotFound(); }

            if (usuarioId != llaveDB.UsuarioId) { return Forbid(); }

            if (actualizarLlaveDTO.ActualizarLlave) { _servicioLlaves.GenerarLlave(); }

            llaveDB.Activa = actualizarLlaveDTO.Activa;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
