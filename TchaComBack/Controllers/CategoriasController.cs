using Microsoft.AspNetCore.Mvc;
using TchaComBack.Data;
using TchaComBack.Models;
using TchaComBack.Repositories;

namespace TchaComBack.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ApplicationDbContext db;

        public CategoriasController(ApplicationDbContext db)
        {
            this.db = db;
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
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(CategoriaModel categoria)
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

                categoria.DataCriacao = DateTime.Now;
                db.Categorias.Add(categoria);
                db.SaveChanges();

                TempData["MensagemSucesso"] = "Categoria criada com sucesso!";
                return RedirectToAction("Index", "Setores");
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível criar a categoria. Detalhes do erro: {erro.Message}";
                return RedirectToAction("Index", "Setores");
            }
        }
    }
}
