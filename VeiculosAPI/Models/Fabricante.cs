using System.ComponentModel.DataAnnotations;

namespace VeiculosAPI.Models
{
    public class Fabricante
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Nome { get; set; }

        // Relacionamento com Veiculo
        public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
    }
}