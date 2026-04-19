using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeiculosAPI.Data;
using VeiculosAPI.Models;

namespace VeiculosAPI.Controllers
{
    /// <summary>
    /// Controller para gerenciar Pagamentos de alugueis.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PagamentosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PagamentosController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém a lista de todos os pagamentos com seus alugueis.
        /// </summary>
        /// <returns>Lista de pagamentos</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Pagamento>>> GetPagamentos()
        {
            return await _context.Pagamentos.Include(p => p.Aluguel).ToListAsync();
        }

        /// <summary>
        /// Obtém um pagamento específico pelo ID.
        /// </summary>
        /// <param name="id">ID do pagamento</param>
        /// <returns>Dados do pagamento com aluguel</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Pagamento>> GetPagamento(int id)
        {
            var pagamento = await _context.Pagamentos.Include(p => p.Aluguel).FirstOrDefaultAsync(p => p.Id == id);

            if (pagamento == null)
            {
                return NotFound();
            }

            return pagamento;
        }

        /// <summary>
        /// Filtro 4: Obtém pagamentos de um cliente específico.
        /// Join: Cliente-Aluguel-Pagamento (aninhado com ThenInclude)
        /// </summary>
        /// <param name="clienteId">ID do cliente</param>
        /// <returns>Lista de pagamentos do cliente</returns>
        [HttpGet("por-cliente/{clienteId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Pagamento>>> GetPagamentosPorCliente(int clienteId)
        {
            var pagamentos = await _context.Pagamentos
                .Include(p => p.Aluguel)
                .ThenInclude(a => a.Cliente)
                .Where(p => p.Aluguel.ClienteId == clienteId)
                .ToListAsync();

            return pagamentos;
        }

        /// <summary>
        /// Filtro 5: Obtém o total de pagamentos para um cliente específico.
        /// Join: Cliente-Aluguel-Pagamento com agregação (SUM)
        /// </summary>
        /// <param name="clienteId">ID do cliente</param>
        /// <returns>Total de pagamentos do cliente</returns>
        [HttpGet("total-por-cliente/{clienteId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<decimal>> GetTotalPagamentosPorCliente(int clienteId)
        {
            var total = await _context.Pagamentos
                .Include(p => p.Aluguel)
                .Where(p => p.Aluguel.ClienteId == clienteId)
                .SumAsync(p => p.Valor);

            return total;
        }

        /// <summary>
        /// Atualiza os dados de um pagamento.
        /// </summary>
        /// <param name="id">ID do pagamento a ser atualizado</param>
        /// <param name="pagamento">Dados do pagamento atualizados</param>
        /// <returns>Sem conteúdo se bem-sucedido</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutPagamento(int id, Pagamento pagamento)
        {
            if (id != pagamento.Id)
            {
                return BadRequest();
            }

            _context.Entry(pagamento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PagamentoExists(id))
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
        /// Cria um novo pagamento.
        /// </summary>
        /// <param name="pagamento">Dados do novo pagamento</param>
        /// <returns>Dados do pagamento criado</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Pagamento>> PostPagamento(Pagamento pagamento)
        {
            _context.Pagamentos.Add(pagamento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPagamento", new { id = pagamento.Id }, pagamento);
        }

        /// <summary>
        /// Deleta um pagamento específico.
        /// </summary>
        /// <param name="id">ID do pagamento a ser deletado</param>
        /// <returns>Sem conteúdo se bem-sucedido</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePagamento(int id)
        {
            var pagamento = await _context.Pagamentos.FindAsync(id);
            if (pagamento == null)
            {
                return NotFound();
            }

            _context.Pagamentos.Remove(pagamento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PagamentoExists(int id)
        {
            return _context.Pagamentos.Any(e => e.Id == id);
        }
    }
}