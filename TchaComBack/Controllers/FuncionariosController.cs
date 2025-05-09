using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using TchaComBack.Data;
using TchaComBack.Models;
using TchaComBack.Repositories;

namespace TchaComBack.Controllers
{
    public class FuncionariosController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IFuncionariosRepositorio funcionariosRepositorio;

        public FuncionariosController(ApplicationDbContext db, IFuncionariosRepositorio _funcionariosRepositorio)
        {
            this.db = db;
            this.funcionariosRepositorio = _funcionariosRepositorio;
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

        public IActionResult Index(int id, string searchString, string cargo, char? ativo)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            var setor = db.Setores.FirstOrDefault(s => s.Id == id);
            if (setor == null)
            {
                TempData["MensagemErro"] = $"{dbconsult.NomeCompleto}, o setor selecionado não foi encontrado";
                return RedirectToAction("Index", "Setores");
            }

            if (setor.Ativo != 'S')
            {
                TempData["MensagemErro"] = $"{dbconsult.NomeCompleto}, o setor selecionado está inativo e não pode ser acessado.";
                return RedirectToAction("Index", "Setores");
            }

            var funcionariosQuery = db.Funcionarios
                .AsNoTracking()
                .Include(f => f.RacaNav)
                .Include(f => f.EstadoCivilNav)
                .Where(f => f.SetorId == id);

            // Filtro por nome
            if (!string.IsNullOrEmpty(searchString))
            {
                funcionariosQuery = funcionariosQuery.Where(f => EF.Functions.Like(f.Nome, $"%{searchString}%"));
            }

            // Filtro por cargo
            if (!string.IsNullOrEmpty(cargo))
            {
                funcionariosQuery = funcionariosQuery.Where(f => f.Cargo == cargo);
            }

            // Filtro por status
            if (ativo.HasValue)
            {
                funcionariosQuery = funcionariosQuery.Where(f => f.Ativo == ativo.Value);
            }

            var funcionarios = funcionariosQuery.ToList();

            var viewModel = new FuncionariosPorSetorViewModel
            {
                SetorId = setor.Id,
                NomeSetor = setor.Nome,
                Funcionarios = funcionarios,
                QuantidadeFuncAtivos = funcionarios.Count(f => f.Ativo == 'S'),
                QuantidadeFuncInativos = funcionarios.Count(f => f.Ativo == 'N')
            };

            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;
            ViewBag.SearchString = searchString;
            ViewBag.Cargo = cargo;
            ViewBag.Ativo = ativo;

            // Lista distinta de cargos dos funcionários desse setor
            ViewBag.CargosOpcoes = new SelectList(db.Funcionarios
                .Where(f => f.SetorId == id)
                .Select(f => f.Cargo)
                .Distinct()
                .ToList());

            ViewBag.StatusOpcoes = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "S", Text = "Ativos" },
                new SelectListItem { Value = "N", Text = "Inativos" }
            }, "Value", "Text", ativo);

            return View(viewModel);
        }

        public IActionResult Funcionarios(string searchString, string setor, char? ativo)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                              .AsNoTracking()
                              .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            var funcionariosQuery = db.Funcionarios
                                      .AsNoTracking()
                                      .Include(f => f.RacaNav)
                                      .Include(f => f.EstadoCivilNav)
                                      .Include(f => f.Setor)
                                      .Where(f => f.UsuarioId == idUsuario);

            // Filtro por nome
            if (!string.IsNullOrEmpty(searchString))
            {
                funcionariosQuery = funcionariosQuery.Where(f => EF.Functions.Like(f.Nome, $"%{searchString}%"));
            }

            // Filtro por setor
            if (!string.IsNullOrEmpty(setor))
            {
                funcionariosQuery = funcionariosQuery.Where(f => f.Setor.Nome == setor);
            }

            // Filtro por status
            if (ativo.HasValue)
            {
                funcionariosQuery = funcionariosQuery.Where(f => f.Ativo == ativo.Value);
            }

            var funcionarios = funcionariosQuery.ToList();

            var viewModel = new FuncionariosViewModel
            {
                NomeSetor = "Todos Funcionários",
                Funcionarios = funcionarios,
                QuantidadeFuncAtivos = funcionarios.Count(f => f.Ativo == 'S'),
                QuantidadeFuncInativos = funcionarios.Count(f => f.Ativo == 'N')
            };

            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;
            ViewBag.SearchString = searchString;
            ViewBag.Setor = setor;
            ViewBag.Ativo = ativo;

            // Lista distinta de setores dos funcionários desse usuário
            ViewBag.SetoresOpcoes = new SelectList(db.Funcionarios
                .Where(f => f.UsuarioId == idUsuario)
                .Include(f => f.Setor)
                .Select(f => f.Setor.Nome)
                .Distinct()
                .ToList());

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
        public IActionResult Cadastrar(FuncionariosModel func)
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
                    return RedirectToAction("Index", "Funcionarios", new { id = func.SetorId });
                }

                func.UsuarioId = sessionIdUsuario;
                func = funcionariosRepositorio.Cadastrar(func);

                TempData["MensagemSucesso"] = "Funcionário cadastrado com sucesso!";
                return RedirectToAction("Index", "Funcionarios", new { id = func.SetorId });
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível cadastrar o funcionário. Detalhes do erro: {erro.Message}";
                return RedirectToAction("Index", "Funcionarios");
            }
        }

        public IActionResult Desativar(int id, int setorId)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            try
            {
                bool desativado = funcionariosRepositorio.Desativar(id);

                if (desativado)
                {
                    TempData["MensagemSucesso"] = "Funcionário desativado com sucesso!";
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao desativar o funcionário!";
                }

                if (setorId == 0)
                {
                    return RedirectToAction("Funcionarios", "Funcionarios");
                }

                return Redirect($"/Funcionarios/Index/{setorId}");
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível desativar o funcionário. Detalhes do erro: {erro.Message}";

                if (setorId == 0)
                {
                    return RedirectToAction("Funcionarios", "Funcionarios");
                }

                return Redirect($"/Funcionarios/Index/{setorId}");
            }
        }

        public IActionResult Reativar(int id, int setorId)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            try
            {
                bool reativado = funcionariosRepositorio.Reativar(id);

                if (reativado)
                {
                    TempData["MensagemSucesso"] = "Funcionário reativado com sucesso!";
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao reativar o funcionário!";
                }

                if(setorId == 0)
                {
                    return RedirectToAction("Funcionarios", "Funcionarios");
                }

                return Redirect($"/Funcionarios/Index/{setorId}");
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível reativar o funcionário. Detalhes do erro: {erro.Message}";

                if (setorId == 0)
                {
                    return RedirectToAction("Funcionarios", "Funcionarios");
                }

                return Redirect($"/Funcionarios/Index/{setorId}");
            }
        }
    }
}