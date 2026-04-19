using System.ComponentModel.DataAnnotations;

namespace VeiculosAPI.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public required string Nome { get; set; }

        [Required]
        [StringLength(14)] // CPF formato 000.000.000-00
        public required string Cpf { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public required string Email { get; set; }

        // Relacionamento com Aluguel
        public ICollection<Aluguel> Alugueis { get; set; } = new List<Aluguel>();
    }
}