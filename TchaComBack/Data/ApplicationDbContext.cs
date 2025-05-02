using Microsoft.EntityFrameworkCore;
using TchaComBack.Models;

namespace TchaComBack.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<UsuariosModel> Usuarios { get; set; }

        public DbSet<FuncionariosModel> Funcionarios { get; set; }

        public DbSet<RacaModel> Raca { get; set; }

        public DbSet<EstadoCivilModel> EstadoCivil { get; set; }

        public DbSet<SetoresModel> Setores { get; set; }

        public DbSet<CategoriaModel> Categorias { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FuncionariosModel>()
                .HasOne(f => f.RacaNav)
                .WithMany(r => r.Funcionarios)
                .HasForeignKey(f => f.Raca)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FuncionariosModel>()
                .HasOne(f => f.EstadoCivilNav)
                .WithMany(e => e.Funcionarios)
                .HasForeignKey(f => f.EstadoCivil)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FuncionariosModel>()
                .HasOne(f => f.Setor)
                .WithMany(s => s.Funcionarios)
                .HasForeignKey(f => f.SetorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SetoresModel>()
                .HasOne(s => s.Categoria)
                .WithMany(c => c.Setores)
                .HasForeignKey(s => s.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
