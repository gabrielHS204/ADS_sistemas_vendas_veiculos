using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeiculosAPI.Models
{
    public class Veiculo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Modelo { get; set; }

        [Required]
        public int AnoFabricacao { get; set; }

        [Required]
        public int Quilometragem { get; set; }

        // Chave estrangeira para Fabricante
        [ForeignKey("Fabricante")]
        public int FabricanteId { get; set; }
        public required Fabricante Fabricante { get; set; }

        // Relacionamento com Aluguel
        public ICollection<Aluguel> Alugueis { get; set; } = new List<Aluguel>();
    }
}