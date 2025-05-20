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

        var usuario = new UsuariosModel
        {
            Id = dbconsult.Id,
            NomeCompleto = dbconsult.NomeCompleto,
            Email = dbconsult.Email,
            TipoPerfil = dbconsult.TipoPerfil,
            DataCadastro = dbconsult.DataCadastro,
            DataHoraEsqueceuSenha = dbconsult.DataHoraEsqueceuSenha
        };

        int totalDeFuncionarios;
        int totalDeSetores;
        int totalDeUsuarios;

        if (usuario.TipoPerfil == 1)
        {
            totalDeFuncionarios = _db.Funcionarios.Count(f => f.Ativo == 'S');
            totalDeSetores = _db.Setores.Count(s => s.Ativo == 'S');
            totalDeUsuarios = _db.Usuarios.Count(u => u.Ativo == 'S');
        }
        else
        {
            // Coordenador: totais restritos ao setor vinculado
            var setorId = dbconsult.SetorId;

            // Funcionários ativos no setor do coordenador
            totalDeFuncionarios = _db.Funcionarios.Count(f => f.Ativo == 'S' && f.SetorId == setorId);

            // Se o setor estiver ativo, conta 1, senão zero
            totalDeSetores = _db.Setores.Any(s => s.Id == setorId && s.Ativo == 'S') ? 1 : 0;

            totalDeUsuarios = _db.Usuarios.Count(u => u.Ativo == 'S' && u.SetorId == setorId);
        }

        int totalFuncionariosAntes = _db.Funcionarios.Count(f => f.Ativo == 'S');
        int totalFuncionariosAgora = totalFuncionariosAntes; 

        string chaveInicial = "PorcentagemAumentoFuncionarios";
        double porcentagemAumento = 0;
        _cache.TryGetValue(chaveInicial, out porcentagemAumento);


        int totalSetoresAntes = _db.Setores.Count(f => f.Ativo == 'S');
        int totalSetoresAgora = totalSetoresAntes; 

        string chaveInicialSetores = "PorcentagemAumentoSetores";
        double porcentagemAumentoSetores = 0;
        _cache.TryGetValue(chaveInicialSetores, out porcentagemAumentoSetores);


        int totalUsuariosAntes = _db.Setores.Count(f => f.Ativo == 'S');
        int totalUsuariosAgora = totalSetoresAntes; 

        string chaveInicialUsuarios = "PorcentagemAumentoUsuarios";
        double porcentagemAumentoUsuarios = 0;
        _cache.TryGetValue(chaveInicialUsuarios, out porcentagemAumentoUsuarios);

        var viewModel = new HomeViewModel
        {
            Usuario = usuario,
            TotalDeFuncionarios = totalDeFuncionarios,
            TotalDeSetores = totalDeSetores,
            TotalDeUsuarios = totalDeUsuarios,
            PorcentagemAumentoFuncionarios = porcentagemAumento,
            PorcentagemAumentoSetores = porcentagemAumentoSetores,
            PorcentagemAumentoUsuarios = porcentagemAumentoUsuarios
        };

        GraficoFuncionarios(viewModel);
        GraficoSetores(viewModel);
        GraficoUsuarios(viewModel);
        GraficoQuantidadeFuncionariosPorSexo(viewModel);
        GraficoSetoresComMaiotQuantidadeDeFuncionarios(viewModel);

        ViewBag.nomeCompleto = dbconsult.NomeCompleto;
        ViewBag.email = dbconsult.Email;
        ViewBag.tipoPerfil = dbconsult.TipoPerfil;

        return View(viewModel);
    }

    private void GraficoFuncionarios(HomeViewModel viewModel)
    {
        var idUsuario = HttpContext.Session.GetInt32("idUsuario");
        if (idUsuario == null) return;

        var usuarioLogado = _db.Usuarios.FirstOrDefault(u => u.Id == idUsuario);
        if (usuarioLogado == null) return;

        var nomesSetores = new List<string>();
        var quantidadeAtivos = new List<int>();
        var quantidadeInativos = new List<int>();

        if (usuarioLogado.TipoPerfil == 1)
        {
            var setores = _db.Setores.ToList();

            foreach (var setor in setores)
            {
                nomesSetores.Add(setor.Nome);

                var ativos = _db.Funcionarios.Count(f => f.SetorId == setor.Id && f.Ativo == 'S');
                var inativos = _db.Funcionarios.Count(f => f.SetorId == setor.Id && f.Ativo == 'N');

                quantidadeAtivos.Add(ativos);
                quantidadeInativos.Add(inativos);
            }
        }
        else if (usuarioLogado.TipoPerfil == 2)
        {
            var setorIdUsuario = usuarioLogado.SetorId;

            var setor = _db.Setores.FirstOrDefault(s => s.Id == setorIdUsuario);
            if (setor != null)
            {
                nomesSetores.Add(setor.Nome);

                var ativos = _db.Funcionarios.Count(f => f.SetorId == setor.Id && f.Ativo == 'S');
                var inativos = _db.Funcionarios.Count(f => f.SetorId == setor.Id && f.Ativo == 'N');

                quantidadeAtivos.Add(ativos);
                quantidadeInativos.Add(inativos);
            }
        }

        viewModel.NomesSetores = nomesSetores;
        viewModel.QuantidadeFuncionariosAtivos = quantidadeAtivos;
        viewModel.QuantidadeFuncionariosInativos = quantidadeInativos;
    }

    private void GraficoSetores(HomeViewModel viewModel)
    {
        var idUsuario = HttpContext.Session.GetInt32("idUsuario");
        if (idUsuario == null) return;

        var usuarioLogado = _db.Usuarios.FirstOrDefault(u => u.Id == idUsuario);
        if (usuarioLogado == null) return;

        var nomesCategorias = new List<string>();
        var quantidadeSetorAtivo = new List<int>();
        var quantidadeSetorInativo = new List<int>();

        if (usuarioLogado.TipoPerfil == 1)
        {
            var categorias = _db.Categorias.ToList();

            foreach (var categoria in categorias)
            {
                nomesCategorias.Add(categoria.Nome);

                var setorAtivo = _db.Setores.Count(s => s.Ativo == 'S' && s.CategoriaId == categoria.Id);
                var setorInativo = _db.Setores.Count(s => s.Ativo == 'N' && s.CategoriaId == categoria.Id);

                quantidadeSetorAtivo.Add(setorAtivo);
                quantidadeSetorInativo.Add(setorInativo);
            }
        }
        else if (usuarioLogado.TipoPerfil == 2)
        {
            var setorUsuario = _db.Setores.FirstOrDefault(s => s.Id == usuarioLogado.SetorId);
            if (setorUsuario != null)
            {
                var categoria = _db.Categorias.FirstOrDefault(c => c.Id == setorUsuario.CategoriaId);
                if (categoria != null)
                {
                    nomesCategorias.Add(categoria.Nome);

                    var setorAtivo = _db.Setores.Count(s => s.Ativo == 'S' && s.CategoriaId == categoria.Id);
                    var setorInativo = _db.Setores.Count(s => s.Ativo == 'N' && s.CategoriaId == categoria.Id);

                    quantidadeSetorAtivo.Add(setorAtivo);
                    quantidadeSetorInativo.Add(setorInativo);
                }
            }
        }

        viewModel.NomesCategorias = nomesCategorias;
        viewModel.QuantidadeSetorAtivos = quantidadeSetorAtivo;
        viewModel.QuantidadeSetorInativos = quantidadeSetorInativo;
    }


    private void GraficoUsuarios(HomeViewModel viewModel)
    {
        var idUsuario = HttpContext.Session.GetInt32("idUsuario");
        if (idUsuario == null) return;

        var usuarioLogado = _db.Usuarios.FirstOrDefault(u => u.Id == idUsuario);
        if (usuarioLogado == null) return;

        var nomesPerfis = new List<string>();
        var quantidadeUsuariosAtivosPorPerfil = new List<int>();
        var quantidadeUsuariosInativosPorPerfil = new List<int>();

        if (usuarioLogado.TipoPerfil == 1)
        {
            var usuariosPorPerfil = _db.Usuarios
                .GroupBy(u => u.TipoPerfil)
                .ToList();

            foreach (var grupo in usuariosPorPerfil)
            {
                string nomePerfil = grupo.Key switch
                {
                    1 => "Administrador",
                    2 => "Coordenador",
                    _ => "Outro"
                };

                int ativos = grupo.Count(u => u.Ativo == 'S');
                int inativos = grupo.Count(u => u.Ativo == 'N');

                nomesPerfis.Add(nomePerfil);
                quantidadeUsuariosAtivosPorPerfil.Add(ativos);
                quantidadeUsuariosInativosPorPerfil.Add(inativos);
            }
        }
        else if (usuarioLogado.TipoPerfil == 2)
        {
            var usuariosDoSetor = _db.Usuarios
                .Where(u => u.SetorId == usuarioLogado.SetorId)
                .GroupBy(u => u.TipoPerfil)
                .ToList();

            foreach (var grupo in usuariosDoSetor)
            {
                string nomePerfil = grupo.Key switch
                {
                    1 => "Administrador",
                    2 => "Coordenador",
                    _ => "Outro"
                };

                int ativos = grupo.Count(u => u.Ativo == 'S');
                int inativos = grupo.Count(u => u.Ativo == 'N');

                nomesPerfis.Add(nomePerfil);
                quantidadeUsuariosAtivosPorPerfil.Add(ativos);
                quantidadeUsuariosInativosPorPerfil.Add(inativos);
            }
        }

        viewModel.NomesPerfis = nomesPerfis;
        viewModel.QuantidadeUsuariosAtivosPorPerfil = quantidadeUsuariosAtivosPorPerfil;
        viewModel.QuantidadeUsuariosInativosPorPerfil = quantidadeUsuariosInativosPorPerfil;
    }


    private void GraficoQuantidadeFuncionariosPorSexo(HomeViewModel viewModel)
    {
        var idUsuario = HttpContext.Session.GetInt32("idUsuario");
        if (idUsuario == null) return;

        var usuarioLogado = _db.Usuarios.FirstOrDefault(u => u.Id == idUsuario);
        if (usuarioLogado == null) return;

        int quantidadeFuncionarioMasculino;
        int quantidadeFuncionarioFeminino;

        if (usuarioLogado.TipoPerfil == 1)
        {
            quantidadeFuncionarioMasculino = _db.Funcionarios.Count(f => f.Sexo == 'M');
            quantidadeFuncionarioFeminino = _db.Funcionarios.Count(f => f.Sexo == 'F');
        }
        else
        {
            quantidadeFuncionarioMasculino = _db.Funcionarios.Count(f => f.Sexo == 'M' && f.SetorId == usuarioLogado.SetorId);
            quantidadeFuncionarioFeminino = _db.Funcionarios.Count(f => f.Sexo == 'F' && f.SetorId == usuarioLogado.SetorId);
        }

        viewModel.LabelsSexoFuncionarios = new List<string> { "Masculino", "Feminino" };
        viewModel.QuantidadeFuncionariosPorSexo = new List<int> { quantidadeFuncionarioMasculino, quantidadeFuncionarioFeminino };
    }


    private void GraficoSetoresComMaiotQuantidadeDeFuncionarios(HomeViewModel viewModel)
    {
        var idUsuario = HttpContext.Session.GetInt32("idUsuario");
        if (idUsuario == null) return;

        var usuarioLogado = _db.Usuarios.FirstOrDefault(u => u.Id == idUsuario);
        if (usuarioLogado == null) return;

        List<string> labelsSetores;
        List<int> quantidadeFuncionarios;

        if (usuarioLogado.TipoPerfil == 1)
        {
            var rankingSetores = _db.Setores
                .Select(s => new
                {
                    NomeSetor = s.Nome,
                    QuantidadeFuncionarios = s.Funcionarios.Count()
                })
                .OrderByDescending(s => s.QuantidadeFuncionarios)
                .ToList();

            labelsSetores = rankingSetores.Select(s => s.NomeSetor).ToList();
            quantidadeFuncionarios = rankingSetores.Select(s => s.QuantidadeFuncionarios).ToList();
        }
        else
        {
            var setor = _db.Setores.FirstOrDefault(s => s.Id == usuarioLogado.SetorId);

            if (setor != null)
            {
                labelsSetores = new List<string> { setor.Nome };
                quantidadeFuncionarios = new List<int> { _db.Funcionarios.Count(f => f.SetorId == setor.Id) };
            }
            else
            {
                labelsSetores = new List<string>();
                quantidadeFuncionarios = new List<int>();
            }
        }

        viewModel.LabelsRankingSetores = labelsSetores;
        viewModel.QuantidadeFuncionariosRankingSetores = quantidadeFuncionarios;
    }


}
