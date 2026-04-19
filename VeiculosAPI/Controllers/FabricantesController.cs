using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeiculosAPI.Data;
using VeiculosAPI.Models;

namespace VeiculosAPI.Controllers
{
    /// <summary>
    /// Controller para gerenciar Fabricantes (marcas de veículos).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class FabricantesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FabricantesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém a lista de todos os fabricantes.
        /// </summary>
        /// <returns>Lista de fabricantes</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Fabricante>>> GetFabricantes()
        {
            return await _context.Fabricantes.ToListAsync();
        }

        /// <summary>
        /// Obtém um fabricante específico pelo ID.
        /// </summary>
        /// <param name="id">ID do fabricante</param>
        /// <returns>Dados do fabricante</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Fabricante>> GetFabricante(int id)
        {
            var fabricante = await _context.Fabricantes.FindAsync(id);

            if (fabricante == null)
            {
                return NotFound();
            }

            return fabricante;
        }

        /// <summary>
        /// Atualiza os dados de um fabricante.
        /// </summary>
        /// <param name="id">ID do fabricante a ser atualizado</param>
        /// <param name="fabricante">Dados do fabricante atualizados</param>
        /// <returns>Sem conteúdo se bem-sucedido</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutFabricante(int id, Fabricante fabricante)
        {
            if (id != fabricante.Id)
            {
                return BadRequest();
            }

            _context.Entry(fabricante).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FabricanteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Cria um novo fabricante.
        /// </summary>
        /// <param name="fabricante">Dados do novo fabricante</param>
        /// <returns>Dados do fabricante criado</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Fabricante>> PostFabricante(Fabricante fabricante)
        {
            _context.Fabricantes.Add(fabricante);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFabricante", new { id = fabricante.Id }, fabricante);
        }

        /// <summary>
        /// Deleta um fabricante específico.
        /// </summary>
        /// <param name="id">ID do fabricante a ser deletado</param>
        /// <returns>Sem conteúdo se bem-sucedido</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFabricante(int id)
        {
            var fabricante = await _context.Fabricantes.FindAsync(id);
            if (fabricante == null)
            {
                return NotFound();
            }

            _context.Fabricantes.Remove(fabricante);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FabricanteExists(int id)
        {
            return _context.Fabricantes.Any(e => e.Id == id);
        }
    }
}