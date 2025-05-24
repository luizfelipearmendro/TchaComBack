using Microsoft.EntityFrameworkCore;
using TchaComBack.Data;
using TchaComBack.Models;

namespace TchaComBack.Repositories
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

        public List<FuncionariosModel> BuscarTodosFuncionarios(int UsuarioResponsavelId )
        {
            return db.Funcionarios
                     .AsNoTracking()
                     .Include(f => f.Setor)
                     .Where(f => f.UsuarioResponsavelId  == UsuarioResponsavelId )
                     .ToList();
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
            var funcExistente = db.Funcionarios.AsNoTracking().FirstOrDefault(f => f.Id == func.Id);

            if (funcExistente == null) throw new System.Exception("Houve um erro na atualização do funcionário!");

            // Atualizar apenas os campos necessários
            funcExistente.Nome = func.Nome;
            funcExistente.DataNascimento = func.DataNascimento;
            funcExistente.Sexo = func.Sexo;
            funcExistente.Raca = func.Raca;
            funcExistente.EstadoCivil = func.EstadoCivil;
            funcExistente.NomeMae = func.NomeMae;
            funcExistente.Naturalidade = func.Naturalidade;
            funcExistente.Endereco = func.Endereco;
            funcExistente.CidadeResidencia = func.CidadeResidencia;
            funcExistente.Email = func.Email;
            funcExistente.Celular = func.Celular;
            funcExistente.SetorId = func.SetorId;
            funcExistente.Cargo = func.Cargo;
            funcExistente.Salario = func.Salario;
            funcExistente.DataIngresso = func.DataIngresso;
            funcExistente.DiasTrabalhados = func.DiasTrabalhados;
            funcExistente.Ativo = func.Ativo;
            funcExistente.DataAtualizacao = DateTime.Now;

            db.Funcionarios.Update(funcExistente);
            db.SaveChanges();

            return funcExistente;
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
