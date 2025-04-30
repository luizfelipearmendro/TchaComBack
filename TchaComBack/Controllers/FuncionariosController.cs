//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Identity.Client;
//using TchaComBack.Data;
//using TchaComBack.Models;
//using TchaComBack.Repositories;

//namespace TchaComBack.Controllers
//{
//    public class FuncionariosController : Controller
//    {
//        private readonly ApplicationDbContext db;
//        private readonly IFuncionariosRepositorio funcionariosRepositorio;

//        public FuncionariosController(ApplicationDbContext db, IFuncionariosRepositorio _funcionariosRepositorio)
//        {
//            this.db = db;
//            this.funcionariosRepositorio = _funcionariosRepositorio;
//        }

//        public int sessionIdUsuario
//        {
//            get
//            {
//                int sessionIdUsuario = 0;
//                if (HttpContext.Session.GetInt32("Id") != null)
//                    sessionIdUsuario = (int)HttpContext.Session.GetInt32("Id");
//                return sessionIdUsuario;
//            }
//        }

//        public IActionResult Index(int id)
//        {
//            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
//            if (idUsuario == null) return RedirectToAction("Index", "Login");

//            var dbconsult = db.Usuarios
//                .AsNoTracking()
//                .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

//            if (dbconsult == null) return RedirectToAction("Index", "Login");

//            var sessionIdUsuario = dbconsult.Id;

//            var setor = db.Setores.FirstOrDefault(s => s.Id == id);
//            if (setor == null)
//            {
//                TempData["MensagemErro"] = $"{dbconsult.NomeCompleto}, o setor selecionado não foi encontrado";
//                return RedirectToAction("Setores","Index");
//            }

//            var funcionarios = db.Funcionarios
//                .Where(f => f.SetorId == id)
//                .ToList();

//            var funcionariosInativos = db.Funcionarios
//                .Where(f => f.SetorId == id && f.Ativo == 'N')
//                .ToList();

//            var viewModel = new FuncionariosPorSetorViewModel
//            {
//                SetorId = setor.Id,
//                NomeSetor = setor.Nome,
//                Funcionarios = funcionarios,
//                Quantidade = funcionarios.Where(f => f.Ativo == 'S').Count()
//            };

//            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
//            ViewBag.Email = dbconsult.Email;
//            ViewBag.TipoPerfil = dbconsult.TipoPerfil;
//            ViewBag.FuncInativos = funcionariosInativos.Count();

//            return View(viewModel);
//        }
//    }
//}





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
    }
}
