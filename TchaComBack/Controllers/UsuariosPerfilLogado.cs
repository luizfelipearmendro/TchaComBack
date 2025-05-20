using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TchaComBack.Data;
using TchaComBack.Helper;
using TchaComBack.Models;

namespace TchaComBack.Controllers
{
    public class UsuariosPerfilLogado : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IMemoryCache _cache;
        public UsuariosPerfilLogado(ApplicationDbContext _db, IMemoryCache cache)
        {
            db = _db;
            _cache = cache;
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

        public IActionResult Index(string searchString, int? tipoPerfil, char? ativo, int pagina = 1, int itensPorPagina = 6)
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

            var totalUsuarios = usuariosQuery.Count();
            var totalPaginas = (int)Math.Ceiling(totalUsuarios / (double)itensPorPagina);

            var funcionariosPaginados = usuariosQuery
                .Skip((pagina - 1) * itensPorPagina)
                .Take(itensPorPagina)
                .ToList();

            var viewModel = new UsuariosViewModel
            {
                QtdUsuariosAtivos = usuariosQuery.Count(u => u.Ativo == 'S'),
                QtdUsuariosInativos = usuariosQuery.Count(u => u.Ativo == 'N'),
                Usuarios = funcionariosPaginados,
                PaginaAtual = pagina,
                TotalPaginas = totalPaginas

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

                int totalAntes = db.Usuarios.Count(f => f.Ativo == 'S') - 1;
                int totalDepois = db.Usuarios.Count(f => f.Ativo == 'S');

                double porcentagemVariacao = 0;

                if (totalAntes > 0)
                {
                    porcentagemVariacao = ((double)(totalDepois - totalAntes) / totalAntes) * 100;
                }
                else if (totalDepois > 0)
                {
                    porcentagemVariacao = 100;
                }

                string cacheKey = "PorcentagemAumentoUsuarios";

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(1));

                _cache.Set(cacheKey, porcentagemVariacao, cacheEntryOptions);

                TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso!";
                return RedirectToAction("Index", "UsuariosPerfilLogado");
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AtualizarSenha()
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;

            var usuario = db.Usuarios.FirstOrDefault(u => u.Id == sessionIdUsuario);

            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;

            if (usuario != null)
            {
                var viewModel = new AtualizarSenhaViewModel
                {
                    Id = usuario.Id,
                    Hash = usuario.Hash
                };
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult AtualizarSenha(AtualizarSenhaViewModel model)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;

            var usuario = db.Usuarios.FirstOrDefault(u => u.Id == model.Id && u.Hash == model.Hash);

            if (usuario != null)
            {
                if (model.NovaSenha == model.ConfirmarSenha)
                {
                    usuario.Senha = Utilitarios.GerarHashSenha(model.NovaSenha, usuario.Salt);
                    usuario.Hash = Utilitarios.GeradorHash();
                    db.SaveChanges();

                    TempData["MensagemSucesso"] = "Senha atualizada com sucesso!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }
    }
}
