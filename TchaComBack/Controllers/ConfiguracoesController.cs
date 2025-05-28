using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TchaComBack.Data;
using TchaComBack.Models;

namespace TchaComBack.Controllers
{
    public class ConfiguracoesController : Controller
    {
        private readonly ApplicationDbContext db;
        public ConfiguracoesController(ApplicationDbContext _db)
        {
            db = _db;
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

        public IActionResult Index()
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                              .AsNoTracking()
                              .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            var configuracoes = db.Configuracoes.ToList();

            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;

            return View(configuracoes);
        }

        public IActionResult Criar()
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                              .AsNoTracking()
                              .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            ViewData["Title"] = "Nova Configuração";

            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;

            ViewBag.DiasSemana = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Domingo" },
                new SelectListItem { Value = "1", Text = "Segunda-feira" },
                new SelectListItem { Value = "2", Text = "Terça-feira" },
                new SelectListItem { Value = "3", Text = "Quarta-feira" },
                new SelectListItem { Value = "4", Text = "Quinta-feira" },
                new SelectListItem { Value = "5", Text = "Sexta-feira" },
                new SelectListItem { Value = "6", Text = "Sábado" }
            };

            return View(new ConfiguracoesModel());
        }

        [HttpPost]
        public IActionResult Criar(ConfiguracoesModel model)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                              .AsNoTracking()
                              .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));
            
            if (dbconsult == null) return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;

            if (ModelState.IsValid)
            {
                model.Ativo = false;
                model.CriadoPor = sessionIdUsuario;
                db.Configuracoes.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Editar(int id)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                              .AsNoTracking()
                              .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            var config = db.Configuracoes.Find(id);
            if (config == null) return NotFound();

            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;
            ViewBag.DiasSemana = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Domingo" },
                new SelectListItem { Value = "1", Text = "Segunda-feira" },
                new SelectListItem { Value = "2", Text = "Terça-feira" },
                new SelectListItem { Value = "3", Text = "Quarta-feira" },
                new SelectListItem { Value = "4", Text = "Quinta-feira" },
                new SelectListItem { Value = "5", Text = "Sexta-feira" },
                new SelectListItem { Value = "6", Text = "Sábado" }
            };

            return View(config);
        }

        [HttpPost]
        public IActionResult Editar(ConfiguracoesModel model)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                              .AsNoTracking()
                              .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                db.Configuracoes.Update(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Ativar(int id)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                              .AsNoTracking()
                              .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            var config = db.Configuracoes.FirstOrDefault(c => c.Id == id);
            if (config == null)
            {
                TempData["Erro"] = "Configuração não encontrada.";
                return RedirectToAction("Index");
            }

            config.Ativo = true;
            db.SaveChanges();

            TempData["MensagemSucesso"] = "Configuração ativada com sucesso!";
            return RedirectToAction("Index");
        }

        public IActionResult Desativar(int id)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                              .AsNoTracking()
                              .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            var config = db.Configuracoes.FirstOrDefault(c => c.Id == id);
            if (config == null)
            {
                TempData["Erro"] = "Configuração não encontrada.";
                return RedirectToAction("Index");
            }

            config.Ativo = false;
            db.SaveChanges();

            TempData["MensagemSucesso"] = "Configuração desativada com sucesso!";
            return RedirectToAction("Index");
        }
    }
}