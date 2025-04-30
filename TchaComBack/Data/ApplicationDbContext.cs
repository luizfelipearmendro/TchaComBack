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
    }
}
