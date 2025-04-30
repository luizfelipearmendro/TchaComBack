using Microsoft.AspNetCore.Mvc;
using TCBSistemaDeControle.Data;
using TCBSistemaDeControle.Models;
using TCBSistemaDeControle.Helper;

namespace TCBSistemaDeControle.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext db;

        public LoginController(ApplicationDbContext _db)
        {
            db = _db;
        }   

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string senha)
        {
            var usuario = db.Usuarios.FirstOrDefault(u => u.Email == email);

            try
            {
                if (usuario != null)
                {
                    var senhaHash = Utilitarios.GerarHashSenha(senha, usuario.Salt);

                    if (senhaHash == usuario.Senha)
                    {
                        string hash = Utilitarios.GeradorHash();

                        usuario.Hash = hash;
                        db.SaveChanges();

                        HttpContext.Session.SetInt32("idUsuario", usuario.Id);
                        HttpContext.Session.SetString("hash", hash);

                        TempData["MensagemSucesso"] = $"Seja bem-vindo {usuario.NomeCompleto}";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["MensagemErro"] = $"E-mail ou senha incorretos!";
                        return View("Index");
                    }
                }

                TempData["MensagemErro"] = $"Ops, usuário não encontrado!";
                return View("Index");
            }
            catch (Exception e)
            {
                TempData["MensagemErro"] = $"Ops, não conseguimos realizar seu login, tente novamente. Detalhe do erro: {e.Message}";
                return RedirectToAction("Index");
            }
        }

        public string idUsuario { get; set; }
        public string hash { get; set; }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "LandingPage");
        }
    }
}
