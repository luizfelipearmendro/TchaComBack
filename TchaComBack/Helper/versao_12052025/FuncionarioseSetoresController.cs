//---------------------- funcionariosController antes da att de filtrar por tipo de acesso

//﻿using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Identity.Client;
//using System;
//using TchaComBack.Data;
//using TchaComBack.Models;
//using TchaComBack.Repositories;

//namespace TchaComBack.Controllers
//{
//    public class FuncionariosController : Controller
//    {
//        private readonly ApplicationDbContext db;
//        private readonly IFuncionariosRepositorio funcionariosRepositorio;

//        public FuncionariosController(ApplicationDbContext db, IFuncionariosRepositorio _funcionariosRepositorio)
//        {
//            this.db = db;
//            this.funcionariosRepositorio = _funcionariosRepositorio;
//        }

//        public int sessionIdUsuario
//        {
//            get
//            {
//                int sessionIdUsuario = 0;
//                if (HttpContext.Session.GetInt32("Id") != null)
//                    sessionIdUsuario = (int)HttpContext.Session.GetInt32("Id");
//                return sessionIdUsuario;
//            }
//        }

//        public IActionResult Index(int id, string searchString, string cargo, char? ativo)
//        {
//            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
//            if (idUsuario == null) return RedirectToAction("Index", "Login");

//            var dbconsult = db.Usuarios
//                .AsNoTracking()
//                .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

//            if (dbconsult == null) return RedirectToAction("Index", "Login");

//            var setor = db.Setores.FirstOrDefault(s => s.Id == id);
//            if (setor == null)
//            {
//                TempData["MensagemErro"] = $"{dbconsult.NomeCompleto}, o setor selecionado não foi encontrado";
//                return RedirectToAction("Index", "Setores");
//            }

//            if (setor.Ativo != 'S')
//            {
//                TempData["MensagemErro"] = $"{dbconsult.NomeCompleto}, o setor selecionado está inativo e não pode ser acessado.";
//                return RedirectToAction("Index", "Setores");
//            }

//            var funcionariosQuery = db.Funcionarios
//                .AsNoTracking()
//                .Include(f => f.RacaNav)
//                .Include(f => f.EstadoCivilNav)
//                .Where(f => f.SetorId == id);

//            // Filtro por nome
//            if (!string.IsNullOrEmpty(searchString))
//            {
//                funcionariosQuery = funcionariosQuery.Where(f => EF.Functions.Like(f.Nome, $"%{searchString}%"));
//            }

//            // Filtro por cargo
//            if (!string.IsNullOrEmpty(cargo))
//            {
//                funcionariosQuery = funcionariosQuery.Where(f => f.Cargo == cargo);
//            }

//            // Filtro por status
//            if (ativo.HasValue)
//            {
//                funcionariosQuery = funcionariosQuery.Where(f => f.Ativo == ativo.Value);
//            }

//            var funcionarios = funcionariosQuery.ToList();

//            var viewModel = new FuncionariosPorSetorViewModel
//            {
//                SetorId = setor.Id,
//                NomeSetor = setor.Nome,
//                Funcionarios = funcionarios,
//                QuantidadeFuncAtivos = funcionarios.Count(f => f.Ativo == 'S'),
//                QuantidadeFuncInativos = funcionarios.Count(f => f.Ativo == 'N')
//            };

//            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
//            ViewBag.Email = dbconsult.Email;
//            ViewBag.TipoPerfil = dbconsult.TipoPerfil;
//            ViewBag.SearchString = searchString;
//            ViewBag.Cargo = cargo;
//            ViewBag.Ativo = ativo;

//            // Lista distinta de cargos dos funcionários desse setor
//            ViewBag.CargosOpcoes = new SelectList(db.Funcionarios
//                .Where(f => f.SetorId == id)
//                .Select(f => f.Cargo)
//                .Distinct()
//                .ToList());

//            ViewBag.Setores = db.Setores
//                                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Nome })
//                                .ToList();

//            ViewBag.StatusOpcoes = new SelectList(new List<SelectListItem>
//            {
//                new SelectListItem { Value = "S", Text = "Ativos" },
//                new SelectListItem { Value = "N", Text = "Inativos" }
//            }, "Value", "Text", ativo);

//            return View(viewModel);
//        }

//        public IActionResult Funcionarios(string searchString, string setor, char? ativo)
//        {
//            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
//            if (idUsuario == null) return RedirectToAction("Index", "Login");

//            var dbconsult = db.Usuarios
//                              .AsNoTracking()
//                              .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

//            if (dbconsult == null) return RedirectToAction("Index", "Login");

//            var funcionariosQuery = db.Funcionarios
//                                      .AsNoTracking()
//                                      .Include(f => f.RacaNav)
//                                      .Include(f => f.EstadoCivilNav)
//                                      .Include(f => f.Setor)
//                                      .Where(f => f.UsuarioId == idUsuario);

//            // Filtro por nome
//            if (!string.IsNullOrEmpty(searchString))
//            {
//                funcionariosQuery = funcionariosQuery.Where(f => EF.Functions.Like(f.Nome, $"%{searchString}%"));
//            }

//            // Filtro por setor
//            if (!string.IsNullOrEmpty(setor))
//            {
//                funcionariosQuery = funcionariosQuery.Where(f => f.Setor.Nome == setor);
//            }

//            // Filtro por status
//            if (ativo.HasValue)
//            {
//                funcionariosQuery = funcionariosQuery.Where(f => f.Ativo == ativo.Value);
//            }

//            var funcionarios = funcionariosQuery.ToList();

//            var viewModel = new FuncionariosViewModel
//            {
//                NomeSetor = "Todos Funcionários",
//                Funcionarios = funcionarios,
//                QuantidadeFuncAtivos = funcionarios.Count(f => f.Ativo == 'S'),
//                QuantidadeFuncInativos = funcionarios.Count(f => f.Ativo == 'N')
//            };

//            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
//            ViewBag.Email = dbconsult.Email;
//            ViewBag.TipoPerfil = dbconsult.TipoPerfil;
//            ViewBag.SearchString = searchString;
//            ViewBag.Setor = setor;
//            ViewBag.Ativo = ativo;

//            // Lista distinta de setores dos funcionários desse usuário
//            ViewBag.SetoresOpcoes = new SelectList(db.Funcionarios
//                .Where(f => f.UsuarioId == idUsuario)
//                .Include(f => f.Setor)
//                .Select(f => f.Setor.Nome)
//                .Distinct()
//                .ToList());

//            ViewBag.StatusOpcoes = new SelectList(new List<SelectListItem>
//            {
//                new SelectListItem { Value = "S", Text = "Ativos" },
//                new SelectListItem { Value = "N", Text = "Inativos" }
//            }, "Value", "Text", ativo);

//            return View(viewModel);
//        }


//        public IActionResult Cadastrar()
//        {
//            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
//            if (idUsuario == null) return RedirectToAction("Index", "Login");

//            var dbconsult = db.Usuarios.Find(idUsuario);
//            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
//                return RedirectToAction("Index", "Login");

//            var sessionIdUsuario = dbconsult.Id;

//            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
//            ViewBag.Email = dbconsult.Email;
//            ViewBag.TipoPerfil = dbconsult.TipoPerfil;

//            return View();
//        }

//        public IActionResult CadastrarSemSetorEspec()
//        {
//            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
//            if (idUsuario == null) return RedirectToAction("Index", "Login");

//            var dbconsult = db.Usuarios.Find(idUsuario);
//            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
//                return RedirectToAction("Index", "Login");

//            var sessionIdUsuario = dbconsult.Id;

//            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
//            ViewBag.Email = dbconsult.Email;
//            ViewBag.TipoPerfil = dbconsult.TipoPerfil;

//            ViewBag.Setores = db.Setores
//                                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Nome })
//                                .ToList();

//            return View();
//        }

//        [HttpPost]
//        public IActionResult Cadastrar(FuncionariosModel func)
//        {
//            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
//            if (idUsuario == null) return RedirectToAction("Index", "Login");

//            var dbconsult = db.Usuarios.Find(idUsuario);
//            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
//                return RedirectToAction("Index", "Login");

//            var sessionIdUsuario = dbconsult.Id;

//            try
//            {
//                if (!ModelState.IsValid)
//                {
//                    TempData["MensagemErro"] = "Dados inválidos!";
//                    return RedirectToAction("Index", "Funcionarios", new { id = func.SetorId });
//                }

//                func.UsuarioId = sessionIdUsuario;
//                func = funcionariosRepositorio.Cadastrar(func);

//                TempData["MensagemSucesso"] = "Funcionário cadastrado com sucesso!";
//                return RedirectToAction("Index", "Funcionarios", new { id = func.SetorId });
//            }
//            catch (System.Exception erro)
//            {
//                TempData["MensagemErro"] = $"Ops, não foi possível cadastrar o funcionário. Detalhes do erro: {erro.Message}";
//                return RedirectToAction("Index", "Funcionarios");
//            }
//        }

//        public IActionResult Desativar(int id, int setorId)
//        {
//            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
//            if (idUsuario == null) return RedirectToAction("Index", "Login");

//            var dbconsult = db.Usuarios.Find(idUsuario);
//            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
//                return RedirectToAction("Index", "Login");

//            try
//            {
//                bool desativado = funcionariosRepositorio.Desativar(id);

//                if (desativado)
//                {
//                    TempData["MensagemSucesso"] = "Funcionário desativado com sucesso!";
//                }
//                else
//                {
//                    TempData["MensagemErro"] = "Erro ao desativar o funcionário!";
//                }

//                if (setorId == 0)
//                {
//                    return RedirectToAction("Funcionarios", "Funcionarios");
//                }

//                return Redirect($"/Funcionarios/Index/{setorId}");
//            }
//            catch (System.Exception erro)
//            {
//                TempData["MensagemErro"] = $"Ops, não foi possível desativar o funcionário. Detalhes do erro: {erro.Message}";

//                if (setorId == 0)
//                {
//                    return RedirectToAction("Funcionarios", "Funcionarios");
//                }

//                return Redirect($"/Funcionarios/Index/{setorId}");
//            }
//        }

//        public IActionResult Reativar(int id, int setorId)
//        {
//            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
//            if (idUsuario == null) return RedirectToAction("Index", "Login");

//            var dbconsult = db.Usuarios.Find(idUsuario);
//            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
//                return RedirectToAction("Index", "Login");

//            try
//            {
//                bool reativado = funcionariosRepositorio.Reativar(id);

//                if (reativado)
//                {
//                    TempData["MensagemSucesso"] = "Funcionário reativado com sucesso!";
//                }
//                else
//                {
//                    TempData["MensagemErro"] = "Erro ao reativar o funcionário!";
//                }

//                if(setorId == 0)
//                {
//                    return RedirectToAction("Funcionarios", "Funcionarios");
//                }

//                return Redirect($"/Funcionarios/Index/{setorId}");
//            }
//            catch (System.Exception erro)
//            {
//                TempData["MensagemErro"] = $"Ops, não foi possível reativar o funcionário. Detalhes do erro: {erro.Message}";

//                if (setorId == 0)
//                {
//                    return RedirectToAction("Funcionarios", "Funcionarios");
//                }

//                return Redirect($"/Funcionarios/Index/{setorId}");
//            }
//        }

//        public IActionResult Editar(int id)
//        {
//            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
//            if (idUsuario == null) return RedirectToAction("Index", "Login");

//            var dbconsult = db.Usuarios.Find(idUsuario);
//            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
//                return RedirectToAction("Index", "Login");

//            var sessionIdUsuario = dbconsult.Id;

//            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
//            ViewBag.Email = dbconsult.Email;
//            ViewBag.TipoPerfil = dbconsult.TipoPerfil;

//            FuncionariosModel func = funcionariosRepositorio.ListarPorId(id);
//            return View(func);
//        }

//        [HttpPost]
//        public IActionResult Editar(FuncionariosModel func, int setorId, string setorNome)
//        {
//            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
//            if (idUsuario == null) return RedirectToAction("Index", "Login");

//            var dbconsult = db.Usuarios.Find(idUsuario);
//            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
//                return RedirectToAction("Index", "Login");

//            var sessionIdUsuario = dbconsult.Id;

//            try
//            {
//                if (!ModelState.IsValid)
//                {
//                    TempData["MensagemErro"] = "Dados inválidos!";

//                    if (setorId == 0)
//                    {
//                        return RedirectToAction("Funcionarios", "Funcionarios");
//                    }

//                    return Redirect($"/Funcionarios/Index/{setorId}");
//                }

//                func.UsuarioId = sessionIdUsuario;
//                func = funcionariosRepositorio.Editar(func);

//                TempData["MensagemSucesso"] = "Funcionário(a) atualizado(a) com sucesso!";

//                if (setorId == 0)
//                {
//                    return RedirectToAction("Funcionarios", "Funcionarios");
//                }

//                return Redirect($"/Funcionarios/Index/{setorId}");
//            }
//            catch (System.Exception erro)
//            {
//                TempData["MensagemErro"] = $"Ops, não foi possível atualizar o(a) funcionário(a). Detalhes do erro: {erro.Message}";
//                return RedirectToAction("Index", "Funcionarios");
//            }
//        }

//        [HttpPost]
//        public IActionResult Mover(int id, int setorId)
//        {
//            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
//            if (idUsuario == null) return RedirectToAction("Index", "Login");

//            var dbconsult = db.Usuarios.Find(idUsuario);
//            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
//                return RedirectToAction("Index", "Login");

//            try
//            {
//                var funcExistente = db.Funcionarios.AsNoTracking().FirstOrDefault(f => f.Id == id);

//                if (funcExistente == null) throw new Exception("Houve um erro na atualização do funcionário!");

//                if (funcExistente.Ativo == 'N')
//                {
//                    TempData["MensagemErro"] = "Não é possivel mover um funcionário inativo.";
 
//                    if (setorId == 0)
//                        return RedirectToAction("Funcionarios", "Funcionarios");

//                    return Redirect($"/Funcionarios/Index/{setorId}");
//                }
//                funcExistente.SetorId = setorId;

//                db.Funcionarios.Update(funcExistente);
//                db.SaveChanges();

//                TempData["MensagemSucesso"] = $"Funcionário(a) movido(a) com sucesso!";
//                if (setorId == 0)
//                {
//                    return RedirectToAction("Funcionarios", "Funcionarios");
//                }

//                return Redirect($"/Funcionarios/Index/{setorId}");
//            }
//            catch (Exception erro)
//            {
//                TempData["MensagemErro"] = $"Erro ao mover funcionário: {erro.Message}";
//                if (setorId == 0)
//                {
//                    return RedirectToAction("Funcionarios", "Funcionarios");
//                }

//                return Redirect($"/Funcionarios/Index/{setorId}");
//            }
//        }
//    }
//}



//---------------------- setoresController antes da att de filtrar por tipo de acesso


//﻿using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using TchaComBack.Data;
//using TchaComBack.Models;
//using TchaComBack.Repositories;
//using static System.Runtime.InteropServices.JavaScript.JSType;

//namespace TchaComBack.Controllers
//{
//    public class SetoresController : Controller
//    {
//        private readonly ApplicationDbContext db;
//        private readonly ISetoresRepositorio setoresRepositorio;

//        public SetoresController(ApplicationDbContext db, ISetoresRepositorio _setoresRepositorio)
//        {
//            this.db = db;
//            setoresRepositorio = _setoresRepositorio;
//        }

//        public int sessionIdUsuario
//        {
//            get
//            {
//                int sessionIdUsuario = 0;
//                if (HttpContext.Session.GetInt32("Id") != null)
//                    sessionIdUsuario = (int)HttpContext.Session.GetInt32("Id");
//                return sessionIdUsuario;
//            }
//        }

//        public IActionResult Index(string searchString, int? categoriaId, [FromQuery] char? ativo)
//        {
//            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
//            if (idUsuario == null) return RedirectToAction("Index", "Login");

//            var dbconsult = db.Usuarios
//                .AsNoTracking()
//                .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

//            if (dbconsult == null) return RedirectToAction("Index", "Login");

//            var sessionIdUsuario = dbconsult.Id;

//            // Consulta inicial dos setores
//            var setoresQuery = db.Setores
//                                 .AsNoTracking()
//                                 .Include(s => s.Categoria)
//                                 .Where(s => s.UsuarioId == sessionIdUsuario);

//            // Aplica o filtro de busca por nome, descrição ou responsável
//            if (!string.IsNullOrEmpty(searchString))
//            {
//                setoresQuery = setoresQuery.Where(s =>
//                    EF.Functions.Like(s.Nome, $"%{searchString}%") ||
//                    EF.Functions.Like(s.Descricao, $"%{searchString}%") ||
//                    EF.Functions.Like(s.ResponsavelSetor, $"%{searchString}%") ||
//                    EF.Functions.Like(s.EmailResponsavelSetor, $"%{searchString}%")
//                );
//            }

//            // Aplica o filtro por categoria
//            if (categoriaId.HasValue)
//            {
//                setoresQuery = setoresQuery.Where(s => s.CategoriaId == categoriaId.Value);
//            }

//            // Aplica o filtro por status (ativo/inativo)
//            if (ativo.HasValue)
//            {
//                setoresQuery = setoresQuery.Where(s => s.Ativo == ativo.Value);
//            }

//            // Executa a consulta e ordena os resultados
//            var setoresFiltrados = setoresQuery
//                                   .OrderBy(s => s.Nome)
//                                   .ThenByDescending(s => s.DataCriacao)
//                                   .ToList();

//            // Obtém todas as categorias do banco de dados (independentemente dos filtros)
//            var todasCategorias = db.Categorias
//                                    .AsNoTracking()
//                                    .ToList();


//            // ----------------------------------------------------- quantidade de funcionarios por setor

//            var quantidadeFunc = db.Funcionarios
//                                   .GroupBy(f => f.SetorId)
//                                   .Select(g => new SetoresViewModel
//                                   {
//                                       SetorId = g.Key,
//                                       Quantidade = g.Count()
//                                   })
//                                   .ToList();


//            var viewModel = new SetoresViewModel
//            {
//                Setores = setoresFiltrados,
//                Categorias = todasCategorias,
//                QuantidadePorSetor = quantidadeFunc
//            };

//            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
//            ViewBag.Email = dbconsult.Email;
//            ViewBag.TipoPerfil = dbconsult.TipoPerfil;
//            ViewBag.SearchString = searchString;
//            ViewBag.CategoriaId = categoriaId;
//            ViewBag.Ativo = ativo;

//            ViewBag.Categorias = db.Categorias
//                                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome })
//                                .ToList();

//            // Prepara as opções para os dropdowns
//            ViewBag.CategoriasOpcoes = new SelectList(todasCategorias, "Id", "Nome", categoriaId); // Todas as categorias
//            ViewBag.StatusOpcoes = new SelectList(new List<SelectListItem>
//            {
//                new SelectListItem { Value = "S", Text = "Ativos" },
//                new SelectListItem { Value = "N", Text = "Inativos" }
//            }, "Value", "Text", ativo);

//            return View(viewModel);
//        }

//        public IActionResult Cadastrar()
//        {
//            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
//            if (idUsuario == null) return RedirectToAction("Index", "Login");

//            var dbconsult = db.Usuarios.Find(idUsuario);
//            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
//                return RedirectToAction("Index", "Login");

//            var sessionIdUsuario = dbconsult.Id;

//            ViewBag.Categorias = db.Categorias
//                                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome })
//                                .ToList();

//            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
//            ViewBag.Email = dbconsult.Email;
//            ViewBag.TipoPerfil = dbconsult.TipoPerfil;

//            return View();
//        }


//        [HttpPost]
//        public IActionResult Cadastrar(SetoresModel setor)
//        {
//            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
//            if (idUsuario == null) return RedirectToAction("Index", "Login");

//            var dbconsult = db.Usuarios.Find(idUsuario);
//            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
//                return RedirectToAction("Index", "Login");

//            var sessionIdUsuario = dbconsult.Id;

//            try
//            {
//                if (!ModelState.IsValid)
//                {
//                    TempData["MensagemErro"] = "Dados inválidos!";
//                    return RedirectToAction("Index", "Setores");
//                }

//                setor.UsuarioId = sessionIdUsuario;
//                setor = setoresRepositorio.Cadastrar(setor);

//                TempData["MensagemSucesso"] = "Setor cadastrado com sucesso!";
//                return RedirectToAction("Index", "Setores");
//            }
//            catch (System.Exception erro)
//            {
//                TempData["MensagemErro"] = $"Ops, não foi possível cadastrar o setor. Detalhes do erro: {erro.Message}";
//                return RedirectToAction("Index", "Setores");
//            }
//        }

//        public IActionResult Editar(int id)
//        {
//            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
//            if (idUsuario == null) return RedirectToAction("Index", "Login");

//            var dbconsult = db.Usuarios.Find(idUsuario);
//            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
//                return RedirectToAction("Index", "Login");

//            var sessionIdUsuario = dbconsult.Id;

//            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
//            ViewBag.Email = dbconsult.Email;
//            ViewBag.TipoPerfil = dbconsult.TipoPerfil;

//            SetoresModel setor = setoresRepositorio.ListarPorId(id);
//            return View(setor);
//        }

//        [HttpPost]
//        public IActionResult Editar(SetoresModel setor)
//        {
//            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
//            if (idUsuario == null) return RedirectToAction("Index", "Login");

//            var dbconsult = db.Usuarios.Find(idUsuario);
//            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
//                return RedirectToAction("Index", "Login");

//            var sessionIdUsuario = dbconsult.Id;

//            try
//            {
//                if (!ModelState.IsValid)
//                {
//                    TempData["MensagemErro"] = "Dados inválidos!";
//                    return RedirectToAction("Index", "Setores");
//                }

//                setor.UsuarioId = sessionIdUsuario;
//                setor = setoresRepositorio.Editar(setor);

//                TempData["MensagemSucesso"] = "Setor atualizado com sucesso!";
//                return RedirectToAction("Index", "Setores");
//            }
//            catch (System.Exception erro)
//            {
//                TempData["MensagemErro"] = $"Ops, não foi possível atualizar o setor. Detalhes do erro: {erro.Message}";
//                return RedirectToAction("Index", "Setores");
//            }
//        }

//        public IActionResult Desativar(int id)
//        {
//            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
//            if (idUsuario == null) return RedirectToAction("Index", "Login");

//            var dbconsult = db.Usuarios.Find(idUsuario);
//            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
//                return RedirectToAction("Index", "Login");

//            var sessionIdUsuario = dbconsult.Id;

//            try
//            {
//                bool desativado = setoresRepositorio.Desativar(id);

//                if (desativado)
//                {
//                    TempData["MensagemSucesso"] = "Setor desativado com sucesso!";
//                }
//                else
//                {
//                    TempData["MensagemErro"] = "Erro ao desativar o setor!";
//                }

//                return RedirectToAction("Index","Setores");
//            }
//            catch (System.Exception erro)
//            {
//                TempData["MensagemErro"] = $"Ops, não foi possível desativar o setor. Detalhes do erro: {erro.Message}";
//                return RedirectToAction("Index", "Setores");
//            }
//        }

//        public IActionResult Reativar(int id)
//        {
//            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
//            if (idUsuario == null) return RedirectToAction("Index", "Login");

//            var dbconsult = db.Usuarios.Find(idUsuario);
//            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
//                return RedirectToAction("Index", "Login");

//            var sessionIdUsuario = dbconsult.Id;

//            try
//            {
//                bool reativado = setoresRepositorio.Reativar(id);

//                if (reativado)
//                {
//                    TempData["MensagemSucesso"] = "Setor reativado com sucesso!";
//                }
//                else
//                {
//                    TempData["MensagemErro"] = "Erro ao reativar o setor!";
//                }

//                return RedirectToAction("Index", "Setores");
//            }
//            catch (System.Exception erro)
//            {
//                TempData["MensagemErro"] = $"Ops, não foi possível reativar o setor. Detalhes do erro: {erro.Message}";
//                return RedirectToAction("Index", "Setores");
//            }
//        }
//    }
//}