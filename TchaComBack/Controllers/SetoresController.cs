﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using TchaComBack.Data;
using TchaComBack.Models;
using TchaComBack.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TchaComBack.Controllers
{
    public class SetoresController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly ISetoresRepositorio setoresRepositorio;
        private readonly IMemoryCache _cache;
        public SetoresController(ApplicationDbContext db, ISetoresRepositorio _setoresRepositorio, IMemoryCache cache)
        {
            this.db = db;
            setoresRepositorio = _setoresRepositorio;
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

        public IActionResult Index(string searchString, int? categoriaId, [FromQuery] char? ativo)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;

            // consulta inicial dos setores com base no id do usuario
            var setoresQuery = db.Setores
                                 .AsNoTracking()
                                 .Include(s => s.Categoria)
                                 .Where(s => s.UsuarioResponsavelId  == sessionIdUsuario);

            // consultas dos filtros..
            if (!string.IsNullOrEmpty(searchString))
            {
                setoresQuery = setoresQuery.Where(s =>
                    EF.Functions.Like(s.Nome, $"%{searchString}%") ||
                    EF.Functions.Like(s.Descricao, $"%{searchString}%") ||
                    EF.Functions.Like(s.ResponsavelSetor, $"%{searchString}%") ||
                    EF.Functions.Like(s.EmailResponsavelSetor, $"%{searchString}%")
                );
            }

            if (categoriaId.HasValue)
            {
                setoresQuery = setoresQuery.Where(s => s.CategoriaId == categoriaId.Value);
            }

            if (ativo.HasValue)
            {
                setoresQuery = setoresQuery.Where(s => s.Ativo == ativo.Value);
            }

            // setores ordenados
            var setoresFiltrados = setoresQuery
                                   .OrderBy(s => s.Nome)
                                   .ThenByDescending(s => s.DataCriacao)
                                   .ToList();

            // apresentar somente o setor vinculado ao coordenador..
            var setorCoord = db.Setores
                                 .AsNoTracking()
                                 .Include(s => s.Categoria)
                                 .Where(s => s.Id == dbconsult.SetorId);


            var todasCategorias = db.Categorias
                                    .AsNoTracking()
                                    .ToList();

            // qtd funcionarios por setor
            var quantidadeFunc = db.Funcionarios
                                   .GroupBy(f => f.SetorId)
                                   .Select(g => new SetoresViewModel
                                   {
                                       SetorId = g.Key,
                                       Quantidade = g.Count()
                                   })
                                   .ToList();


            // viewbags - propriedades dinâmicas que permitem transferir dados do controlador para a view. é útil para passar informações adicionais que não fazem parte do modelo da view..
            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;
            ViewBag.SearchString = searchString;
            ViewBag.CategoriaId = categoriaId;
            ViewBag.Ativo = ativo;

            ViewBag.Categorias = db.Categorias
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome })
                .ToList();

            if(dbconsult.TipoPerfil == 1)
            {
                ViewBag.CategoriasOpcoes = new SelectList(todasCategorias, "Id", "Nome", categoriaId);
            }
            else
            {
                var categoriaDoSetor = db.Setores
                                         .AsNoTracking()
                                         .Where(s => s.Id == dbconsult.SetorId)
                                         .Select(s => s.CategoriaId)
                                         .FirstOrDefault();

                var categoriasCriadasPeloCoord = db.Setores
                                                   .AsNoTracking()
                                                   .Where(s => s.UsuarioResponsavelId  == dbconsult.Id)
                                                   .Select(s => s.CategoriaId)
                                                   .FirstOrDefault();

                var categoriasFiltradas = todasCategorias
                                            .Where(c => c.Id == categoriasCriadasPeloCoord || c.Id == categoriaDoSetor)
                                            .Distinct()
                                            .ToList();

                ViewBag.CategoriasOpcoes = new SelectList(categoriasFiltradas, "Id", "Nome", categoriaId);
            }

            ViewBag.StatusOpcoes = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "S", Text = "Ativos" },
                new SelectListItem { Value = "N", Text = "Inativos" }
            }, "Value", "Text", ativo);

            // condição para qual ViewModel retornar com base no perfil
            if (dbconsult.TipoPerfil == 1)
            {
                var viewModel1 = new SetoresViewModel
                {
                    Setores = setoresFiltrados,
                    Categorias = todasCategorias,
                    QuantidadePorSetor = quantidadeFunc
                };
                return View(viewModel1);
            }
            else
            {
                var viewModel2 = new SetoresViewModel
                {
                    Setores = setorCoord,
                    Categorias = todasCategorias,
                    QuantidadePorSetor = quantidadeFunc
                };
                return View(viewModel2);
            }
        }


        public IActionResult Cadastrar()
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;

            ViewBag.Categorias = db.Categorias
                                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome })
                                .ToList();

            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;

            return View();
        }


        [HttpPost]
        public IActionResult Cadastrar(SetoresModel setor)
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

                setor.UsuarioResponsavelId  = sessionIdUsuario;
                setor = setoresRepositorio.Cadastrar(setor);

                int totalAntes = db.Setores.Count(s => s.Ativo == 'S' && s.Id != setor.Id); // evita contar o novo duas vezes
                int totalDepois = db.Setores.Count(s => s.Ativo == 'S');

                double porcentagemVariacao = 0;

                if (totalAntes > 0)
                {
                    porcentagemVariacao = ((double)(totalDepois - totalAntes) / totalAntes) * 100;
                }
                else if (totalDepois > 0)
                {
                    porcentagemVariacao = 100; 
                }

                string cacheKey = "PorcentagemAumentoSetores";
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(1));

                _cache.Set(cacheKey, porcentagemVariacao, cacheEntryOptions);

                TempData["MensagemSucesso"] = "Setor cadastrado com sucesso!";
                return RedirectToAction("Index", "Setores");
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível cadastrar o setor. Detalhes do erro: {erro.Message}";
                return RedirectToAction("Index", "Setores");
            }
        }

        public IActionResult Editar(int id)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;

            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;

            ViewBag.Categorias = db.Categorias
                                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome })
                                .ToList();

            SetoresModel setor = setoresRepositorio.ListarPorId(id);
            return View(setor);
        }

        [HttpPost]
        public IActionResult Editar(SetoresModel setor)
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

                setor.UsuarioResponsavelId  = sessionIdUsuario;
                setor = setoresRepositorio.Editar(setor);

                TempData["MensagemSucesso"] = "Setor atualizado com sucesso!";
                return RedirectToAction("Index", "Setores");
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível atualizar o setor. Detalhes do erro: {erro.Message}";
                return RedirectToAction("Index", "Setores");
            }
        }

        public IActionResult Desativar(int id)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            var sessionIdUsuario = dbconsult.Id;

            try
            {
                bool desativado = setoresRepositorio.Desativar(id);

                var desativarFuncionariosVinculadosAoSetor = db.Funcionarios.Where(f => f.SetorId == id && f.Ativo == 'S').ToList();

                foreach( var funcionarios  in desativarFuncionariosVinculadosAoSetor)
                {
                    funcionarios.Ativo = 'N';

                }

                db.SaveChanges();

                int totalAntes = db.Setores.Count(s => s.Ativo == 'S');
                int totalDepois = totalAntes - 1;

                double porcentagemVariacao = 0;

                if (totalAntes > 0)
                {
                    porcentagemVariacao = ((double)(totalDepois - totalAntes) / totalAntes) * 100;
                }

                string cacheKey = "PorcentagemAumentoSetores";
                _cache.Set(cacheKey, porcentagemVariacao, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1)));

                if (desativado)
                {
                    TempData["MensagemSucesso"] = "Setor desativado com sucesso!";
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao desativar o setor!";
                }

                return RedirectToAction("Index","Setores");
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível desativar o setor. Detalhes do erro: {erro.Message}";
                return RedirectToAction("Index", "Setores");
            }
        }

        public IActionResult Reativar(int id)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            try
            {
                bool reativado = setoresRepositorio.Reativar(id);

                int totalAntes = db.Setores.Count(s => s.Ativo == 'S');
                int totalDepois = totalAntes + 1;

                double porcentagemVariacao = 0;

                if (totalAntes > 0)
                {
                    porcentagemVariacao = ((double)(totalDepois - totalAntes) / totalAntes) * 100;
                }
                else if (totalDepois > 0)
                {
                    porcentagemVariacao = 100; 
                }

                string cacheKey = "PorcentagemAumentoSetores";
                _cache.Set(cacheKey, porcentagemVariacao, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1)));

                if (reativado)
                {
                    TempData["MensagemSucesso"] = "Setor reativado com sucesso!";
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao reativar o setor.";
                }

                return RedirectToAction("Index", "Setores");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível reativar o setor. Detalhes do erro: {erro.Message}";
                return RedirectToAction("Index", "Setores");
            }
        }
    }
}