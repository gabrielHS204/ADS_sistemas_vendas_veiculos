using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeiculosAPI.Data;
using VeiculosAPI.Models;

namespace VeiculosAPI.Controllers
{
    /// <summary>
    /// Controller para gerenciar Alugueis (locações de veículos).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AlugueisController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AlugueisController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém a lista de todos os alugueis com clientes e veículos.
        /// </summary>
        /// <returns>Lista de alugueis</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Aluguel>>> GetAlugueis()
        {
            return await _context.Alugueis.Include(a => a.Cliente).Include(a => a.Veiculo).ToListAsync();
        }

        /// <summary>
        /// Obtém um aluguel específico pelo ID.
        /// </summary>
        /// <param name="id">ID do aluguel</param>
        /// <returns>Dados do aluguel com cliente e veículo</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Aluguel>> GetAluguel(int id)
        {
            var aluguel = await _context.Alugueis.Include(a => a.Cliente).Include(a => a.Veiculo).FirstOrDefaultAsync(a => a.Id == id);

            if (aluguel == null)
            {
                return NotFound();
            }

            return aluguel;
        }

        /// <summary>
        /// Filtro 1: Obtém alugueis de um cliente específico.
        /// Join: Cliente-Aluguel
        /// </summary>
        /// <param name="clienteId">ID do cliente</param>
        /// <returns>Lista de alugueis do cliente</returns>
        [HttpGet("por-cliente/{clienteId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Aluguel>>> GetAlugueisPorCliente(int clienteId)
        {
            var alugueis = await _context.Alugueis
                .Include(a => a.Cliente)
                .Include(a => a.Veiculo)
                .Where(a => a.ClienteId == clienteId)
                .ToListAsync();

            return alugueis;
        }

        /// <summary>
        /// Filtro 2: Obtém alugueis que ainda não foram devolvidos (ativos).
        /// Join: Aluguel-Veiculo-Cliente
        /// </summary>
        /// <returns>Lista de alugueis ativos</returns>
        [HttpGet("ativos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Aluguel>>> GetAlugueisAtivos()
        {
            var alugueis = await _context.Alugueis
                .Include(a => a.Cliente)
                .Include(a => a.Veiculo)
                .Where(a => a.DataDevolucao == null)
                .ToListAsync();

            return alugueis;
        }

        /// <summary>
        /// Atualiza os dados de um aluguel.
        /// </summary>
        /// <param name="id">ID do aluguel a ser atualizado</param>
        /// <param name="aluguel">Dados do aluguel atualizados</param>
        /// <returns>Sem conteúdo se bem-sucedido</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutAluguel(int id, Aluguel aluguel)
        {
            if (id != aluguel.Id)
            {
                return BadRequest();
            }

            _context.Entry(aluguel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AluguelExists(id))
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
        /// Cria um novo aluguel.
        /// </summary>
        /// <param name="aluguel">Dados do novo aluguel</param>
        /// <returns>Dados do aluguel criado</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Aluguel>> PostAluguel(Aluguel aluguel)
        {
            _context.Alugueis.Add(aluguel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAluguel", new { id = aluguel.Id }, aluguel);
        }

        /// <summary>
        /// Deleta um aluguel específico.
        /// </summary>
        /// <param name="id">ID do aluguel a ser deletado</param>
        /// <returns>Sem conteúdo se bem-sucedido</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAluguel(int id)
        {
            var aluguel = await _context.Alugueis.FindAsync(id);
            if (aluguel == null)
            {
                return NotFound();
            }

            _context.Alugueis.Remove(aluguel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AluguelExists(int id)
        {
            return _context.Alugueis.Any(e => e.Id == id);
        }
    }
}