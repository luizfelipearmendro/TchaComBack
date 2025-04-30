using TCBSistemaDeControle.Models;
using TCBSistemaDeControle.Data;
using TCBSistemaDeControle.Repositories;
using Org.BouncyCastle.Asn1.X509;
using Microsoft.EntityFrameworkCore;

namespace TCBSistemaDeControle.Repositories
{
    public class SetoresRepositorio : ISetoresRepositorio
    {
        private readonly ApplicationDbContext db;

        public SetoresRepositorio(ApplicationDbContext _db)
        {
            this.db = _db;
        }

        public SetoresModel ListarPorId(int id)
        {
            return db.Setores.FirstOrDefault(s => s.Id == id);
        }

        public List<SetoresModel> BuscarTodosSetores(int usuarioId)
        {
            return db.Setores.Where(s => s.UsuarioId == usuarioId).ToList();
        }

        public SetoresModel Cadastrar(SetoresModel setor)
        {
            setor.DataCriacao = DateTime.Now;

            db.Setores.Add(setor);
            db.SaveChanges();

            return setor;
        }

        public SetoresModel Editar(SetoresModel setor)
        {
            var setorExistente = ListarPorId(setor.Id);

            if (setorExistente == null)
                throw new Exception("Setor não encontrado.");

            setorExistente.Nome = setor.Nome;
            setorExistente.Descricao = setor.Descricao;
            setorExistente.ResponsavelSetor = setor.ResponsavelSetor;
            setorExistente.EmailResponsavelSetor = setor.EmailResponsavelSetor;
            setorExistente.SexoResponsavel = setor.SexoResponsavel;
            setorExistente.Localizacao = setor.Localizacao;
            setorExistente.ImagemSetor = setor.ImagemSetor;
            setorExistente.CategoriaId = setor.CategoriaId;
            setorExistente.DataAtualizacao = DateTime.Now;

            db.Setores.Update(setorExistente);
            db.SaveChanges();

            return setorExistente;
        }

        public bool Desativar(int id)
        {
            var setor = ListarPorId(id);

            if (setor == null)
                throw new Exception("Setor não encontrado.");

            setor.Desativar();
            db.Setores.Update(setor);
            db.SaveChanges();

            return true;
        }

        public bool Reativar(int id)
        {
            var setor = ListarPorId(id);

            if (setor == null)
                throw new Exception("Setor não encontrado.");

            if (setor.EstaAtivo())
                return false;

            setor.Reativar();
            db.Setores.Update(setor);
            db.SaveChanges();

            return true;
        }
    }
}