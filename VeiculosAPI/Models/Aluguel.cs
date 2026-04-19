using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeiculosAPI.Models
{
    public class Aluguel
    {
        [Key]
        public int Id { get; set; }

        // Chave estrangeira para Cliente
        [ForeignKey("Cliente")]
        public int ClienteId { get; set; }
        public required Cliente Cliente { get; set; }

        // Chave estrangeira para Veiculo
        [ForeignKey("Veiculo")]
        public int VeiculoId { get; set; }
        public required Veiculo Veiculo { get; set; }

        [Required]
        public DateTime DataInicio { get; set; }

        [Required]
        public DateTime DataFim { get; set; }

        [Required]
        public int QuilometragemInicial { get; set; }

        public int? QuilometragemFinal { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorDiaria { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorTotal { get; set; }

        public DateTime? DataDevolucao { get; set; }

        // Relacionamento com Pagamento
        public ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
    }
}