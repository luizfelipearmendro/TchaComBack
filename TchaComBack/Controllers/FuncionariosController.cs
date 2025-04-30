using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index(int id)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;

            var setor = db.Setores.FirstOrDefault(s => s.Id == id);
            if (setor == null)
            {
                TempData["MensagemErro"] = $"{dbconsult.NomeCompleto}, o setor selecionado não foi encontrado";
                return RedirectToAction("Setores","Index");
            }

            var funcionarios = db.Funcionarios
                .Where(f => f.SetorId == id && f.Ativo == 'S')
                .ToList();


            var viewModel = new FuncionariosPorSetorViewModel
            {
                SetorId = setor.Id,
                NomeSetor = setor.Nome,
                Funcionarios = funcionarios,
                Quantidade = funcionarios.Count()
            };

            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;

            return View(viewModel);
        }
    }
}
