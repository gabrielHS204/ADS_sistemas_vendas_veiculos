using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeiculosAPI.Models
{
    public class Pagamento
    {
        [Key]
        public int Id { get; set; }

        // Chave estrangeira para Aluguel
        [ForeignKey("Aluguel")]
        public int AluguelId { get; set; }
        public required Aluguel Aluguel { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        [Required]
        public DateTime DataPagamento { get; set; }

        [Required]
        [StringLength(50)]
        public required string MetodoPagamento { get; set; } // Ex: Cartão, Dinheiro, Pix
    }
}