using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using TchaComBack.Data;
using TchaComBack.Models;
using Microsoft.Extensions.Caching.Memory;

namespace TchaComBack.Controllers;

public class HomeController : Controller
{
    readonly private ApplicationDbContext _db;
    private readonly IMemoryCache _cache;
    public HomeController(ApplicationDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
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

     

        var viewModel = new HomeViewModel{};

        //GraficoFuncionarios(viewModel);

        ViewBag.nomeCompleto = dbconsult.NomeCompleto;
        ViewBag.email = dbconsult.Email;
        ViewBag.tipoPerfil = dbconsult.TipoPerfil;

        return View(viewModel);
    }

    //private void GraficoFuncionarios(HomeViewModel viewModel)
    //{
    //    var idUsuario = HttpContext.Session.GetInt32("idUsuario");
    //    if (idUsuario == null) return;

    //    var usuarioLogado = _db.Usuarios.FirstOrDefault(u => u.Id == idUsuario);
    //    if (usuarioLogado == null) return;

    //    var nomesSetores = new List<string>();
    //    var quantidadeAtivos = new List<int>();
    //    var quantidadeInativos = new List<int>();

    //    if (usuarioLogado.TipoPerfil == 1)
    //    {
    //        var setores = _db.Setores.ToList();

    //        foreach (var setor in setores)
    //        {
    //            nomesSetores.Add(setor.Nome);

    //            var ativos = _db.Funcionarios.Count(f => f.SetorId == setor.Id && f.Ativo == 'S');
    //            var inativos = _db.Funcionarios.Count(f => f.SetorId == setor.Id && f.Ativo == 'N');

    //            quantidadeAtivos.Add(ativos);
    //            quantidadeInativos.Add(inativos);
    //        }
    //    }
    //    else if (usuarioLogado.TipoPerfil == 2)
    //    {
    //        var setorIdUsuario = usuarioLogado.SetorId;

    //        var setor = _db.Setores.FirstOrDefault(s => s.Id == setorIdUsuario);
    //        if (setor != null)
    //        {
    //            nomesSetores.Add(setor.Nome);

    //            var ativos = _db.Funcionarios.Count(f => f.SetorId == setor.Id && f.Ativo == 'S');
    //            var inativos = _db.Funcionarios.Count(f => f.SetorId == setor.Id && f.Ativo == 'N');

    //            quantidadeAtivos.Add(ativos);
    //            quantidadeInativos.Add(inativos);
    //        }
    //    }

    //    viewModel.NomesSetores = nomesSetores;
    //    viewModel.QuantidadeFuncionariosAtivos = quantidadeAtivos;
    //    viewModel.QuantidadeFuncionariosInativos = quantidadeInativos;
    //}


}
