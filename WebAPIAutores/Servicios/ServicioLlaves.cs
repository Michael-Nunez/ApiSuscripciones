using System;
using System.Threading.Tasks;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Servicios
{
    public class ServicioLlaves
    {
        private readonly ApplicationDbContext _context;

        public ServicioLlaves(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CrearLlave(string usuarioId, TipoLlave tipoLlave)
        {
            var llave = GenerarLlave();

            var llaveApi = new LlaveAPI
            {
                Activa = true,
                Llave = llave,
                TipoLlave = tipoLlave,
                UsuarioId = usuarioId,
            };

            _context.Add(llaveApi);
            await _context.SaveChangesAsync();
        }
        public string GenerarLlave()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
