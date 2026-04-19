using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeiculosAPI.Data;
using VeiculosAPI.Models;

namespace VeiculosAPI.Controllers
{
    /// <summary>
    /// Controller para gerenciar Veículos do sistema de locação.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class VeiculosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VeiculosController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém a lista de todos os veículos com seus fabricantes.
        /// </summary>
        /// <returns>Lista de veículos</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Veiculo>>> GetVeiculos()
        {
            return await _context.Veiculos.Include(v => v.Fabricante).ToListAsync();
        }

        /// <summary>
        /// Obtém um veículo específico pelo ID.
        /// </summary>
        /// <param name="id">ID do veículo</param>
        /// <returns>Dados do veículo com fabricante</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Veiculo>> GetVeiculo(int id)
        {
            var veiculo = await _context.Veiculos.Include(v => v.Fabricante).FirstOrDefaultAsync(v => v.Id == id);

            if (veiculo == null)
            {
                return NotFound();
            }

            return veiculo;
        }

        /// <summary>
        /// Filtro 3: Obtém veículos de um fabricante específico.
        /// Join: Fabricante-Veiculo
        /// </summary>
        /// <param name="fabricanteId">ID do fabricante</param>
        /// <returns>Lista de veículos do fabricante</returns>
        [HttpGet("por-fabricante/{fabricanteId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Veiculo>>> GetVeiculosPorFabricante(int fabricanteId)
        {
            var veiculos = await _context.Veiculos
                .Include(v => v.Fabricante)
                .Where(v => v.FabricanteId == fabricanteId)
                .ToListAsync();

            return veiculos;
        }

        /// <summary>
        /// Atualiza os dados de um veículo.
        /// </summary>
        /// <param name="id">ID do veículo a ser atualizado</param>
        /// <param name="veiculo">Dados do veículo atualizados</param>
        /// <returns>Sem conteúdo se bem-sucedido</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutVeiculo(int id, Veiculo veiculo)
        {
            if (id != veiculo.Id)
            {
                return BadRequest();
            }

            _context.Entry(veiculo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VeiculoExists(id))
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
        /// Cria um novo veículo.
        /// </summary>
        /// <param name="veiculo">Dados do novo veículo</param>
        /// <returns>Dados do veículo criado</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Veiculo>> PostVeiculo(Veiculo veiculo)
        {
            _context.Veiculos.Add(veiculo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVeiculo", new { id = veiculo.Id }, veiculo);
        }

        /// <summary>
        /// Deleta um veículo específico.
        /// </summary>
        /// <param name="id">ID do veículo a ser deletado</param>
        /// <returns>Sem conteúdo se bem-sucedido</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVeiculo(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
            {
                return NotFound();
            }

            _context.Veiculos.Remove(veiculo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VeiculoExists(int id)
        {
            return _context.Veiculos.Any(e => e.Id == id);
        }
    }
}