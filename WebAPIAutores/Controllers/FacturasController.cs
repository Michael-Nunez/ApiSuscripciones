using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/facturas")]
    public class FacturasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FacturasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Pagar(PagarFacturaDTO pagarFacturaDTO)
        {
            var facturaDB = await _context.Facturas
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Id == pagarFacturaDTO.FacturaId);

            if (facturaDB == null) { return NotFound(); }

            if (facturaDB.Pagada) { return BadRequest("La factura ya fue saldada."); }

            // Logica para pagar la factura (pasarela de pago).
            // Nosotros vamos a pretender que el pago fue exitoso.

            facturaDB.Pagada = true;
            await _context.SaveChangesAsync();

            var hayFacturasPendientesVencidas = await _context.Facturas
                .AnyAsync(x => x.UsuarioId == facturaDB.UsuarioId && !x.Pagada && x.FechaLimiteDePago < DateTime.Today);

            if (!hayFacturasPendientesVencidas)
            {
                facturaDB.Usuario.MalaPaga = false;
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
