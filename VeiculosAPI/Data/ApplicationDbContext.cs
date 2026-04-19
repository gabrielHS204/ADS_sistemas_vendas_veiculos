using Microsoft.EntityFrameworkCore;
using VeiculosAPI.Models;

namespace VeiculosAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Fabricante> Fabricantes { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Aluguel> Alugueis { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações adicionais se necessário
            // Ex: restrições únicas, índices, etc.

            // Exemplo: CPF único
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Cpf)
                .IsUnique();

            // Exemplo: Email único
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Email)
                .IsUnique();
        }
    }
}