using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TchaComBack.Data;
using TchaComBack.Helper;
using TchaComBack.Models;

namespace TchaComBack.Controllers
{
    public class UsuariosPerfilLogado : Controller
    {
        private readonly ApplicationDbContext db;

        public UsuariosPerfilLogado(ApplicationDbContext _db)
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

            var sessionIdUsuario = dbconsult.Id;

            var usuariosQuery = db.Usuarios
                                    .OrderBy(u => u.DataCadastro)
                                    .Include(f => f.Setor)
                                    .AsNoTracking()
                                    .ToList();

            var countUsurios = usuariosQuery.ToList();

            var viewModel = new UsuariosViewModel
            {
                QtdUsuariosAtivos = countUsurios.Count(f => f.Ativo == 'S'),
                QtdUsuariosInativos = countUsurios.Count(f => f.Ativo == 'N'),
                Usuarios = usuariosQuery
            };

            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;
            ViewBag.Setores = db.Setores
                                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Nome })
                                .ToList();

            return View(viewModel);
        }

        public IActionResult CadastrarNovoUsuario()
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;


            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;
            ViewBag.Setores = db.Setores
                                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Nome })
                                .ToList();

            return View();
        }

        [HttpPost]
        public IActionResult CadastrarNovoUsuario(UsuariosModel usuario)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;



            var validaEmailExistente = db.Usuarios.Any(u => u.Email == usuario.Email);
            if (validaEmailExistente)
            {
                TempData["MensagemErro"] = "Ops, o e-mail informado já existe!";
                return RedirectToAction("CadastrarNovoUsuario", "Usuarios");
            }

            var salt = Utilitarios.GerarSalt();
            usuario.Salt = salt;

            if (!Utilitarios.SenhaEhForte(usuario.Senha, out string mensagemErro))
            {
                TempData["MensagemErro"] = mensagemErro;
                return RedirectToAction("CadastrarNovoUsuario", "Usuarios");
            }

            usuario.Senha = Utilitarios.GerarHashSenha(usuario.Senha, salt);
            usuario.Hash = Utilitarios.GeradorHash();
            usuario.Confirmado = 0;
            usuario.DataCadastro = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Usuarios.Add(usuario);
                db.SaveChanges();

                TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso!";
                return RedirectToAction("Index", "UsuariosPerfilLogado");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
