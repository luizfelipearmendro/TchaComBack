using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using MimeKit;
using System;
using System.Data;
using TchaComBack.Data;
using TchaComBack.Helper;
using TchaComBack.Models;
using TchaComBack.Repositories;

namespace TchaComBack.Controllers
{
    public class FuncionariosController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IFuncionariosRepositorio funcionariosRepositorio;
        private readonly IMemoryCache _cache;
        public FuncionariosController(ApplicationDbContext db, IFuncionariosRepositorio _funcionariosRepositorio, IMemoryCache cache)
        {
            this.db = db;
            this.funcionariosRepositorio = _funcionariosRepositorio;
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

        public IActionResult Index(int id, string searchString, string cargo, char? ativo, int pagina = 1, int itensPorPagina = 6)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            var setor = db.Setores.FirstOrDefault(s => s.Id == id);
            if (setor == null)
            {
                TempData["MensagemErro"] = $"{dbconsult.NomeCompleto}, o setor selecionado não foi encontrado";
                return RedirectToAction("Index", "Setores");
            }

            if (setor.Ativo != 'S')
            {
                TempData["MensagemErro"] = $"{dbconsult.NomeCompleto}, o setor selecionado está inativo e não pode ser acessado.";
                return RedirectToAction("Index", "Setores");
            }

            var funcionariosQuery = db.Funcionarios
                .AsNoTracking()
                .Include(f => f.RacaNav)
                .Include(f => f.EstadoCivilNav)
                .Where(f => f.SetorId == id);
                //.Where(f => f.UsuarioResponsavelId  == sessionIdUsuario);

            if (!string.IsNullOrEmpty(searchString))
            {
                funcionariosQuery = funcionariosQuery.Where(f => EF.Functions.Like(f.Nome, $"%{searchString}%"));
            }

            if (!string.IsNullOrEmpty(cargo))
            {
                funcionariosQuery = funcionariosQuery.Where(f => f.Cargo == cargo);
            }

            if (ativo.HasValue)
            {
                funcionariosQuery = funcionariosQuery.Where(f => f.Ativo == ativo.Value);
            }

            var totalFuncionarios = funcionariosQuery.Count();
            var totalPaginas = (int)Math.Ceiling(totalFuncionarios / (double)itensPorPagina);

            var funcionariosPaginados = funcionariosQuery
                .Skip((pagina - 1) * itensPorPagina)
                .Take(itensPorPagina)
                .ToList();

            var viewModel = new FuncionariosPorSetorViewModel
            {
                Funcionarios = funcionariosPaginados,
                SetorId = setor.Id,
                NomeSetor = setor.Nome,
                QuantidadeFuncAtivos = funcionariosPaginados.Count(f => f.Ativo == 'S'),
                QuantidadeFuncInativos = funcionariosPaginados.Count(f => f.Ativo == 'N'),
                PaginaAtual = pagina,
                TotalPaginas = totalPaginas
            };

            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;
            ViewBag.SearchString = searchString;
            ViewBag.Cargo = cargo;
            ViewBag.Ativo = ativo;

            // Lista distinta de cargos dos funcionários desse setor
            ViewBag.CargosOpcoes = new SelectList(db.Funcionarios
                                                    .Where(f => f.SetorId == id)
                                                    .Select(f => f.Cargo)
                                                    .Distinct()
                                                    .ToList());

            ViewBag.Setores = db.Setores
                                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Nome })
                                .ToList();

            ViewBag.StatusOpcoes = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "S", Text = "Ativos" },
                new SelectListItem { Value = "N", Text = "Inativos" }
            }, "Value", "Text", ativo);

            return View(viewModel);
        }

        public IActionResult Funcionarios(string searchString, string setor, char? ativo, int pagina = 1, int itensPorPagina = 6)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios
                              .AsNoTracking()
                              .FirstOrDefault(u => u.Id == idUsuario && u.Hash == HttpContext.Session.GetString("hash"));

            if (dbconsult == null) return RedirectToAction("Index", "Login");

            IQueryable<FuncionariosModel> funcionariosQuery = db.Funcionarios
                                                                .AsNoTracking()
                                                                .Include(f => f.RacaNav)
                                                                .Include(f => f.EstadoCivilNav)
                                                                .Include(f => f.Setor);

            // filtrar pelo tipo do perfil
            if (dbconsult.TipoPerfil != 1)
            {
                funcionariosQuery = funcionariosQuery.Where(f => f.SetorId == dbconsult.SetorId);
            }

            // --------------------------------------------------------------------------------- filtros
            if (!string.IsNullOrEmpty(searchString))
            {
                funcionariosQuery = funcionariosQuery.Where(f => EF.Functions.Like(f.Nome, $"%{searchString}%"));
            }

            if (!string.IsNullOrEmpty(setor))
            {
                funcionariosQuery = funcionariosQuery.Where(f => f.Setor.Nome == setor);
            }

            if (ativo.HasValue)
            {
                funcionariosQuery = funcionariosQuery.Where(f => f.Ativo == ativo.Value);
            }
            var totalFuncionarios = funcionariosQuery.Count();
            var totalPaginas = (int)Math.Ceiling(totalFuncionarios / (double)itensPorPagina);

            var funcionariosPaginados = funcionariosQuery
                .Skip((pagina - 1) * itensPorPagina)
                .Take(itensPorPagina)
                .ToList();

            var viewModel = new FuncionariosViewModel
            {
                NomeSetor = "Todos Funcionários",
                Funcionarios = funcionariosPaginados, 
                QuantidadeFuncAtivos = funcionariosPaginados.Count(f => f.Ativo == 'S'),
                QuantidadeFuncInativos = funcionariosPaginados.Count(f => f.Ativo == 'N'),
                PaginaAtual = pagina,
                TotalPaginas = totalPaginas,
            };

            // viewBags
            ViewBag.NomeCompleto = dbconsult.NomeCompleto;
            ViewBag.Email = dbconsult.Email;
            ViewBag.TipoPerfil = dbconsult.TipoPerfil;
            ViewBag.SearchString = searchString;
            ViewBag.Setor = setor;
            ViewBag.Ativo = ativo;

            if (dbconsult.TipoPerfil == 1)
            {
                ViewBag.SetoresOpcoes = new SelectList(db.Setores
                    .Select(s => s.Nome)
                    .Distinct()
                    .ToList());
            }
            else
            {
                var setorCoordenador = db.Setores
                    .Where(s => s.Id == dbconsult.SetorId)
                    .Select(s => s.Nome)
                    .ToList();

                ViewBag.SetoresOpcoes = new SelectList(setorCoordenador);
            }

            ViewBag.StatusOpcoes = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "S", Text = "Ativos" },
                new SelectListItem { Value = "N", Text = "Inativos" }
            }, "Value", "Text", ativo);

            return View(viewModel);
        }

        public IActionResult Cadastrar()
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

            return View();
        }

        public IActionResult CadastrarSemSetorEspec()
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

            if(dbconsult.TipoPerfil == 1)
            {
                ViewBag.Setores = db.Setores
                                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Nome })
                                    .ToList();
            }
            else
            {
                ViewBag.Setores = db.Setores
                                    .Where(s => s.Id == dbconsult.SetorId)
                                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Nome })
                                    .ToList();
            }

            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(FuncionariosModel func)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            try
            {
                if (!ModelState.IsValid)
               {
                    TempData["MensagemErro"] = "Dados inválidos!";
                    return RedirectToAction("Index", "Funcionarios", new { id = func.SetorId });
                }

                if (db.Usuarios.Any(u => u.Email == func.Email))
                {
                    TempData["MensagemErro"] = "Já existe um usuário com este e-mail!";
                    return RedirectToAction("Index", "Funcionarios", new { id = func.SetorId });
                }

                func.UsuarioResponsavelId = dbconsult.Id;
                func = funcionariosRepositorio.Cadastrar(func);

                if (dbconsult.TipoPerfil == 2)
                {
                    CadastrarUsuarioBase(func);
                }

                int totalAntes = db.Funcionarios.Count(f => f.Ativo == 'S') - 1;
                int totalDepois = db.Funcionarios.Count(f => f.Ativo == 'S');

                double porcentagemVariacao = 0;

                if (totalAntes > 0)
                {
                    porcentagemVariacao = ((double)(totalDepois - totalAntes) / totalAntes) * 100;
                }
                else if (totalDepois > 0)
                {
                    porcentagemVariacao = 100;
                }

                string cacheKey = "PorcentagemAumentoFuncionarios";

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(1));

                _cache.Set(cacheKey, porcentagemVariacao, cacheEntryOptions);

                if (dbconsult.TipoPerfil == 1)
                {
                    TempData["MensagemSucesso"] = "Funcionário cadastrado com sucesso!";
                }
                else if (dbconsult.TipoPerfil == 2)
                {
                    TempData["MensagemSucesso"] = "Funcionário e Usuário Base cadastrado com sucesso!";
                }
                return RedirectToAction("Index", "Funcionarios", new { id = func.SetorId });
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível cadastrar o funcionário ou o usuário base. Detalhes do erro: {erro.Message}";
                return RedirectToAction("Index", "Funcionarios");
            }
        }


        public void CadastrarUsuarioBase(FuncionariosModel func)
        {
            string senhaPadrao = "abc123";
            string salt = Utilitarios.GerarSalt();
            string hashSenha = Utilitarios.GerarHashSenha(senhaPadrao, salt);

            var usuarioBase = new UsuariosModel
            {
                Email = func.Email,
                Senha = hashSenha,
                NomeCompleto = func.Nome,
                TipoPerfil = 3,
                SetorId = func.SetorId,
                Matricula = func.Matricula,
                DataCadastro = DateTime.Now,
                UltimoAcesso = DateTime.MinValue,
                DataHoraEsqueceuSenha = DateTime.MinValue,
                Confirmado = 1,
                Hash = Guid.NewGuid().ToString(),
                Salt = salt,
                Ativo = 'S'
            };

            string resetLink = Url.Action("Index", "Login", new { id = usuarioBase.Id }, protocol: HttpContext.Request.Scheme);

            string corpoEmail = $@"
                                            <table style='width:100%; max-width:600px; font-family:Calibri, sans-serif; border:1px solid #ddd; padding:20px;'>
                                                <tr>
                                                    <td style='text-align:center; padding:30px 20px 10px 20px;'>
                                                        <img src='https://i.postimg.cc/pTRwypv7/TCG.png' alt='Logo TCB' width='150' height='80' style='margin-bottom:10px;' />
                                                        <h2 style='margin:0; color: #FFA500; font-size:1.8rem;'>Bem-vindo ao Sistema</h2>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style='padding:10px 0; font-size:1.2rem; color:#333;'>
                                                        Olá <strong>{usuarioBase.NomeCompleto}</strong>,
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style='padding:10px 0; font-size:1rem; color:#333;'>
                                                        Seu acesso ao sistema foi criado com sucesso. Utilize os dados abaixo para fazer login:
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style='padding:10px 0;font-size:1.2rem;'>
                                                        <strong>E-mail:</strong>{usuarioBase.Email}<br/>
                                                        <strong>Senha:</strong> abc123
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style='padding:20px 0; text-align:center;'>
                                                        <a href='{resetLink}' style='background: linear-gradient(90deg,#8A2BE2, #FFA500); color: white; padding:12px 20px; text-decoration:none; border-radius:5px; font-weight:bold;font-size: 1rem;'>Acessar o sistema</a>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style='padding-top:30px; font-size:13px; color:#888; text-align:center;'>
                                                        Se você não reconhece este e-mail, apenas ignore esta mensagem.
                                                    </td>
                                                </tr>
                                            </table>";

            var utilitarios = new Utilitarios();
            utilitarios.EnviarEmail(usuarioBase.Email, "Login do Funcionário", corpoEmail, isHtml: true);

            //TempData["MensagemSucesso"] = "Email com o login do funcionário, enviado com sucesso!";

            var usuarioExistente = db.Usuarios.FirstOrDefault(u => u.Matricula == func.Matricula);
            if (usuarioExistente == null)
            {
                db.Usuarios.Add(usuarioBase);
                db.SaveChanges();
            }
            else
            {
                TempData["MensagemAlerta"] = "Já existe um usuário com esta matrícula.";
            }
        }

        public IActionResult Desativar(int id, int setorId)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            try
            {
                bool desativado = funcionariosRepositorio.Desativar(id);

                int totalAntes = db.Funcionarios.Count(f => f.Ativo == 'S');
                int totalDepois = totalAntes - 1;

                double porcentagemVariacao = 0;

                if (totalAntes > 0)
                {
                    porcentagemVariacao = ((double)(totalDepois - totalAntes) / totalAntes) * 100;
                }

                string cacheKey = "PorcentagemAumentoFuncionarios";
                _cache.Set(cacheKey, porcentagemVariacao, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1)));

                if (desativado)
                {
                    TempData["MensagemSucesso"] = "Funcionário desativado com sucesso!";
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao desativar o funcionário.";
                }

                if (setorId == 0)
                {
                    return RedirectToAction("Funcionarios");
                }
                return Redirect($"/Funcionarios/Index/{setorId}");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível desativar o funcionário. Detalhes do erro: {erro.Message}";
                return Redirect($"/Funcionarios/Index/{setorId}");
            }
        }

        public IActionResult Reativar(int id, int setorId)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            try
            {
                bool reativado = funcionariosRepositorio.Reativar(id);

                int totalAntes = db.Funcionarios.Count(f => f.Ativo == 'S');
                int totalDepois = totalAntes + 1;

                double porcentagemVariacao = 0;

                if (totalAntes > 0)
                {
                    porcentagemVariacao = ((double)(totalDepois - totalAntes) / totalAntes) * 100;
                }
                else
                {
                    porcentagemVariacao = 100; // primeiro funcionário ativo
                }

                string cacheKey = "PorcentagemAumentoFuncionarios";
                _cache.Set(cacheKey, porcentagemVariacao, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1)));

                if (reativado)
                {
                    TempData["MensagemSucesso"] = "Funcionário reativado com sucesso!";
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao reativar o funcionário.";
                }

                if (setorId == 0)
                {
                    return RedirectToAction("Funcionarios");
                }
                return Redirect($"/Funcionarios/Index/{setorId}");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível reativar o funcionário. Detalhes do erro: {erro.Message}";
                return Redirect($"/Funcionarios/Index/{setorId}");
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

            FuncionariosModel func = funcionariosRepositorio.ListarPorId(id);
            return View(func);
        }

        [HttpPost]
        public IActionResult Editar(FuncionariosModel func, int setorId, string setorNome)
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

                    if (setorId == 0)
                    {
                        return RedirectToAction("Funcionarios", "Funcionarios");
                    }

                    return Redirect($"/Funcionarios/Index/{setorId}");
                }

                func.UsuarioResponsavelId  = sessionIdUsuario;
                func = funcionariosRepositorio.Editar(func);

                TempData["MensagemSucesso"] = "Funcionário(a) atualizado(a) com sucesso!";

                if (setorId == 0)
                {
                    return RedirectToAction("Funcionarios", "Funcionarios");
                }

                return Redirect($"/Funcionarios/Index/{setorId}");
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível atualizar o(a) funcionário(a). Detalhes do erro: {erro.Message}";
                return RedirectToAction("Index", "Funcionarios");
            }
        }

        [HttpPost]
        public IActionResult Mover(int id, int setorId)
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            if (idUsuario == null) return RedirectToAction("Index", "Login");

            var dbconsult = db.Usuarios.Find(idUsuario);
            if (dbconsult == null || dbconsult.Hash != HttpContext.Session.GetString("hash"))
                return RedirectToAction("Index", "Login");

            try
            {
                var funcExistente = db.Funcionarios.AsNoTracking().FirstOrDefault(f => f.Id == id);

                if (funcExistente == null) throw new Exception("Houve um erro na atualização do funcionário!");

                if (funcExistente.Ativo == 'N')
                {
                    TempData["MensagemErro"] = "Não é possivel mover um funcionário inativo.";
 
                    if (setorId == 0)
                        return RedirectToAction("Funcionarios", "Funcionarios");

                    return Redirect($"/Funcionarios/Index/{setorId}");
                }
                funcExistente.SetorId = setorId;

                db.Funcionarios.Update(funcExistente);
                db.SaveChanges();

                TempData["MensagemSucesso"] = $"Funcionário(a) movido(a) com sucesso!";
                if (setorId == 0)
                {
                    return RedirectToAction("Funcionarios", "Funcionarios");
                }

                return Redirect($"/Funcionarios/Index/{setorId}");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao mover funcionário: {erro.Message}";
                if (setorId == 0)
                {
                    return RedirectToAction("Funcionarios", "Funcionarios");
                }

                return Redirect($"/Funcionarios/Index/{setorId}");
            }
        }
    }
}