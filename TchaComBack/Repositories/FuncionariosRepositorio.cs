using TCBSistemaDeControle.Data;
using TCBSistemaDeControle.Models;

namespace TCBSistemaDeControle.Repositories
{
    public class FuncionariosRepositorio : IFuncionariosRepositorio
    {
        private readonly ApplicationDbContext db;

        public FuncionariosRepositorio(ApplicationDbContext _db)
        {
            this.db = _db;
        }

        public FuncionariosModel ListarPorId(int id)
        {
            return db.Funcionarios.FirstOrDefault(f => f.Id == id);
        }

        public List<FuncionariosModel> BuscarTodosFuncionarios(int usuarioId)
        {
            return db.Funcionarios.Where(f => f.UsuarioId == usuarioId).ToList();
        }

        public FuncionariosModel Cadastrar(FuncionariosModel func)
        {
            func.DataCadastro = DateTime.Now;

            db.Funcionarios.Add(func);
            db.SaveChanges();

            return func;
        }

        public FuncionariosModel Editar(FuncionariosModel func)
        {
            FuncionariosModel funcEditado = ListarPorId(func.Id);

            if (func == null) throw new System.Exception("Houve um erro na atualização do funcionário!");
            func.Nome = funcEditado.Nome;
            func.DataNascimento = funcEditado.DataNascimento;
            func.Sexo = funcEditado.Sexo;
            func.Raca = funcEditado.Raca;
            func.EstadoCivil = funcEditado.EstadoCivil;
            func.NomeMae = funcEditado.NomeMae;
            func.Naturalidade = funcEditado.Naturalidade;
            func.Endereco = funcEditado.Endereco;
            func.CidadeResidencia = funcEditado.CidadeResidencia;
            func.Email = funcEditado.Email;
            func.Celular = funcEditado.Celular;
            func.SetorId = funcEditado.SetorId;
            func.Cargo = funcEditado.Cargo;
            func.Salario = funcEditado.Salario;
            func.DataIngresso = funcEditado.DataIngresso;
            func.DiasTrabalhados = funcEditado.DiasTrabalhados;
            func.DataCadastro = funcEditado.DataCadastro;
            func.DataAtualizacao = funcEditado.DataAtualizacao;
            func.Ativo = funcEditado.Ativo;
            func.DataAtualizacao = DateTime.Now;

            db.Funcionarios.Update(func);
            db.SaveChanges();

            return func;
        }

        //public List<FuncionariosPorSetorViewModel> ObterFuncionariosPorSetor()
        //{
        //    return db.Funcionarios
        //        .GroupBy(f => f.Setor)
        //        .Select(g => new FuncionariosPorSetorViewModel
        //        {
        //            SetorId = g.Key.Id,
        //            NomeSetor = g.Key.Nome,
        //            Quantidade = g.Count()
        //        })
        //        .ToList();
        //}

        public bool Desativar(int id)
        {
            var func = ListarPorId(id);

            if (func == null)
                throw new Exception("Funcionário não encontrado.");

            func.Desativar();
            db.Funcionarios.Update(func);
            db.SaveChanges();

            return true;
        }

        public bool Reativar(int id)
        {
            var func = ListarPorId(id);

            if (func == null)
                throw new Exception("Funcionário não encontrado.");

            if (func.EstaAtivo())
                return false;

            func.Reativar();
            db.Funcionarios.Update(func);
            db.SaveChanges();

            return true;
        }
    }
}
