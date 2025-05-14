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

        public IActionResult Index(string searchString, int? tipoPerfil, char? ativo)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            var usuariosQuery = db.Usuarios
                .Include(u => u.Setor)
                .AsNoTracking()
                .OrderBy(u => u.DataCadastro)
                .AsQueryable();

         
            if (!string.IsNullOrEmpty(searchString))
            {
                usuariosQuery = usuariosQuery.Where(u => EF.Functions.Like(u.NomeCompleto, $"%{searchString}%"));
            }
            if (tipoPerfil.HasValue)
            {
                usuariosQuery = usuariosQuery.Where(u => u.TipoPerfil == tipoPerfil.Value);
            }

            if (ativo.HasValue)
            {
                usuariosQuery = usuariosQuery.Where(u => u.Ativo == ativo.Value);
            }

            var usuariosFiltrados = usuariosQuery.ToList();

            var viewModel = new UsuariosViewModel
            {
                QtdUsuariosAtivos = usuariosFiltrados.Count(u => u.Ativo == 'S'),
                QtdUsuariosInativos = usuariosFiltrados.Count(u => u.Ativo == 'N'),
                Usuarios = usuariosFiltrados
            };

            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;

          
            ViewBag.TiposPerfilOpcoes = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Administrador" },
                new SelectListItem { Value = "2", Text = "Coordenador" }
            }, "Value", "Text", tipoPerfil);

          
                    ViewBag.StatusOpcoes = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "S", Text = "Ativos" },
                new SelectListItem { Value = "N", Text = "Inativos" }
            }, "Value", "Text", ativo);

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
                return RedirectToAction("Index", "UsuariosPerfilLogado");
            }

            var salt = Utilitarios.GerarSalt();
            usuario.Salt = salt;

            if (!Utilitarios.SenhaEhForte(usuario.Senha, out string mensagemErro))
            {
                TempData["MensagemErro"] = mensagemErro;
                return RedirectToAction("CadastrarNovoUsuario", "UsuariosPerfilLogado");
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
