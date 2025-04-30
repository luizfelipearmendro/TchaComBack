using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TCBSistemaDeControle.Data;
using TCBSistemaDeControle.Models;

namespace TCBSistemaDeControle.Controllers;

public class HomeController : Controller
{
    readonly private ApplicationDbContext _db;

    public HomeController(ApplicationDbContext db)
    {
        _db = db;
    }

    public int sessionidusuario
    {
        get
        {
            int sessionidusuario = 0;
            if (HttpContext.Session.GetInt32("idUsuario") != null)
                sessionidusuario = (int)HttpContext.Session.GetInt32("idUsuario");
            return sessionidusuario;
        }
    }

    public IActionResult Index()
    {
        var idUsuario = HttpContext.Session.GetInt32("idUsuario");
        if (idUsuario == null) return RedirectToAction("Index", "Login");

        var dbconsult = _db.Usuarios.Find(idUsuario);
        if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
            return RedirectToAction("Index", "Login");

        var usuario = new UsuariosModel
        {
            Id = dbconsult.Id,
            NomeCompleto = dbconsult.NomeCompleto,
            Email = dbconsult.Email,
            TipoPerfil = dbconsult.TipoPerfil,
            DataCadastro = dbconsult.DataCadastro,
            DataHoraEsqueceuSenha = dbconsult.DataHoraEsqueceuSenha
        };

        var viewModel = new HomeViewModel
        {
            Usuario = usuario
        };

        var nomeCompleto = dbconsult.NomeCompleto;
        var email = dbconsult.Email;
        var tipoPerfil = dbconsult.TipoPerfil;

        ViewBag.nomeCompleto = nomeCompleto;
        ViewBag.email = email;
        ViewBag.tipoPerfil = tipoPerfil;

        return View(viewModel);
    }
}
