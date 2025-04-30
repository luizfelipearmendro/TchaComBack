using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TCBSistemaDeControle.Data;
using TCBSistemaDeControle.Models;
using TCBSistemaDeControle.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TCBSistemaDeControle.Controllers
{
    public class SetoresController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly ISetoresRepositorio setoresRepositorio;

        public SetoresController(ApplicationDbContext db, ISetoresRepositorio _setoresRepositorio)
        {
            this.db = db;
            setoresRepositorio = _setoresRepositorio;
        }

        public int sessionIdUsuario
        {
            get
            {
                int sessionIdUsuario = 0;
                if (HttpContext.Session.GetInt32("Id") != null)
                    sessionIdUsuario = (int)HttpContext.Session.GetInt32("Id");
                return sessionIdUsuario;
            }
        }

        public IActionResult Index(string searchString, int? categoriaId, [FromQuery] char? ativo)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;

            // Consulta inicial dos setores
            var setoresQuery = db.Setores
                .AsNoTracking()
                .Where(s => s.UsuarioId == sessionIdUsuario);

            // Aplica o filtro de busca por nome, descrição ou responsável
            if (!string.IsNullOrEmpty(searchString))
            {
                setoresQuery = setoresQuery.Where(s =>
                    EF.Functions.Like(s.Nome, $"%{searchString}%") ||
                    EF.Functions.Like(s.Descricao, $"%{searchString}%") ||
                    EF.Functions.Like(s.ResponsavelSetor, $"%{searchString}%") ||
                    EF.Functions.Like(s.EmailResponsavelSetor, $"%{searchString}%")
                );
            }

            // Aplica o filtro por categoria
            if (categoriaId.HasValue)
            {
                setoresQuery = setoresQuery.Where(s => s.CategoriaId == categoriaId.Value);
            }

            // Aplica o filtro por status (ativo/inativo)
            if (ativo.HasValue)
            {
                setoresQuery = setoresQuery.Where(s => s.Ativo == ativo.Value);
            }

            // Executa a consulta e ordena os resultados
            var setoresFiltrados = setoresQuery
                                   .OrderBy(s => s.Nome)
                                   .ThenByDescending(s => s.DataCriacao)
                                   .ToList();

            // Obtém todas as categorias do banco de dados (independentemente dos filtros)
            var todasCategorias = db.Categorias
                                    .AsNoTracking()
                                    .ToList();


            // ----------------------------------------------------- quantidade de funcionarios por setor

            var quantidadeFunc = db.Funcionarios
                                   .GroupBy(f => f.SetorId)
                                   .Select(g => new SetoresViewModel
                                   {
                                       SetorId = g.Key,
                                       Quantidade = g.Count()
                                   })
                                   .ToList();

            var viewModel = new SetoresViewModel
            {
                Setores = setoresFiltrados,
                Categorias = todasCategorias,
                QuantidadePorSetor = quantidadeFunc
            };

            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;
            ViewBag.SearchString = searchString;
            ViewBag.CategoriaId = categoriaId;
            ViewBag.Ativo = ativo;

            // Prepara as opções para os dropdowns
            ViewBag.CategoriasOpcoes = new SelectList(todasCategorias, "Id", "Nome", categoriaId); // Todas as categorias
            ViewBag.StatusOpcoes = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "S", Text = "Ativos" },
                new SelectListItem { Value = "N", Text = "Inativos" }
            }, "Value", "Text", ativo);

            return View(viewModel);
        }

        public IActionResult Cadastrar()
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;

            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;

            return View();
        }


        [HttpPost]
        public IActionResult Cadastrar(SetoresModel setor)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;

            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["MensagemErro"] = "Dados inválidos!";
                    return RedirectToAction("Index", "Setores");
                }

                setor.UsuarioId = sessionIdUsuario;
                setor = setoresRepositorio.Cadastrar(setor);

                TempData["MensagemSucesso"] = "Setor cadastrado com sucesso!";
                return RedirectToAction("Index", "Setores");
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível cadastrar o setor. Detalhes do erro: {erro.Message}";
                return RedirectToAction("Index", "Setores");
            }
        }

        public IActionResult Editar(int id)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;

            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;

            SetoresModel setor = setoresRepositorio.ListarPorId(id);
            return View(setor);
        }

        [HttpPost]
        public IActionResult Atualizar(SetoresModel setor)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;

            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["MensagemErro"] = "Dados inválidos!";
                    return RedirectToAction("Index", "Setores");
                }

                setor.UsuarioId = sessionIdUsuario;
                setor = setoresRepositorio.Editar(setor);

                TempData["MensagemSucesso"] = "Setor atualizado com sucesso!";
                return RedirectToAction("Index", "Setores");
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível atualizar o setor. Detalhes do erro: {erro.Message}";
                return RedirectToAction("Index", "Setores");
            }
        }

        public IActionResult Desativar(int id)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;

            try
            {
                bool desativado = setoresRepositorio.Desativar(id);

                if (desativado)
                {
                    TempData["MensagemSucesso"] = "Setor desativado com sucesso!";
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao desativar o setor!";
                }

                return RedirectToAction("Index","Setores");
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível desativar o setor. Detalhes do erro: {erro.Message}";
                return RedirectToAction("Index", "Setores");
            }
        }

        public IActionResult Reativar(int id)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;

            try
            {
                bool reativado = setoresRepositorio.Reativar(id);

                if (reativado)
                {
                    TempData["MensagemSucesso"] = "Setor reativado com sucesso!";
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao reativar o setor!";
                }

                return RedirectToAction("Index", "Setores");
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível reativar o setor. Detalhes do erro: {erro.Message}";
                return RedirectToAction("Index", "Setores");
            }
        }
    }
}