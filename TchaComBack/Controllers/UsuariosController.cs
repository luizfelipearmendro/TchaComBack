using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MimeKit;
using TchaComBack.Data;
using TchaComBack.Helper;
using TchaComBack.Models;

namespace TchaComBack.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IMemoryCache _cache;

        public UsuariosController(ApplicationDbContext _db, IMemoryCache cache)
        {
            db = _db;
            _cache = cache;
        }   

        public IActionResult Index()
        {
            var idUsuario = HttpContext.Session.GetInt32("idUsuario");
            var hash = HttpContext.Session.GetString("hash");

            if(idUsuario == null || string.IsNullOrEmpty(hash))
            {
                return RedirectToAction("Login", "Index");
            }

            var usuarios = db.Usuarios.ToList();

            return View(usuarios);
        }

        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(UsuariosModel usuario)
        {
            var validaEmailExistente = db.Usuarios.Any(u => u.Email == usuario.Email);
            if (validaEmailExistente)
            {
                TempData["MensagemErro"] = "Ops, o e-mail informado já existe!";
                return RedirectToAction("Index", "Login");
            }

            var salt = Utilitarios.GerarSalt();
            usuario.Salt = salt;

            if (!Utilitarios.SenhaEhForte(usuario.Senha, out string mensagemErro))
            {
                TempData["MensagemErro"] = mensagemErro;
                return RedirectToAction("Index", "Login");
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

                TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso! Aguarde o administrador liberar seu acesso.";
                return RedirectToAction("Index", "Login");
            }

            return RedirectToAction("Index", "Login");
        }

        public void EnviarEmail(string para, string assunto, string mensagemCorpo, bool isHtml = false)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("TchaComBack", "tchacomback@gmail.com"));
            message.To.Add(new MailboxAddress("tchacomback@gmail.com", para));
            message.Subject = assunto;

            var bodyBuilder = new BodyBuilder();

            if (isHtml)
            {
                bodyBuilder.HtmlBody = mensagemCorpo;

                // Se o corpo do e-mail faz referência a "cid:logo", então anexa a imagem
                if (mensagemCorpo.Contains("cid:logo"))
                {
                    var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "TCG.png");
                    var logo = bodyBuilder.LinkedResources.Add(logoPath);
                    logo.ContentId = "logo";
                }
            }
            else
            {
                bodyBuilder.TextBody = mensagemCorpo;
            }

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate("tchacomback@gmail.com", "qark grzc ltgk arsz ");
                    client.Send(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao enviar e-mail: " + ex.Message);
                }
                finally
                {
                    client.Disconnect(true);
                }
            }
        }

        public IActionResult EsqueceuSenha()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EnviarEsqueceuSenha(string email)
        {
            var usuario = db.Usuarios.FirstOrDefault(u => u.Email == email);

            if (usuario != null)
            {
                int primeiroEsqueceuSenha = 0;

                if (usuario.DataHoraEsqueceuSenha == DateTime.MinValue)
                {
                    primeiroEsqueceuSenha = 1;
                    usuario.DataHoraEsqueceuSenha = DateTime.Now;
                    db.SaveChanges();
                }

                int minutos = (int)DateTime.Now.Subtract(usuario.DataHoraEsqueceuSenha).TotalMinutes;

                if (primeiroEsqueceuSenha == 0 && minutos <= 5)
                {
                    TempData["MensagemErro"] = "Foi solicitado recuperação de senha em menos de 5 minutos!";
                    return RedirectToAction("Index", "Login");
                }
                else
                {

                    usuario.Hash = Utilitarios.GeradorHash();
                    usuario.DataHoraEsqueceuSenha = DateTime.Now;
                    db.SaveChanges();

                    string resetLink = Url.Action("AtualizarSenha", "Usuarios", new { id = usuario.Id, hash = usuario.Hash }, protocol: HttpContext.Request.Scheme);

                    string corpoEmail = $@"
                                           <table style='width:100%; max-width:600px; font-family: Calibri, sans-serif; border:1px solid #ddd; padding:20px;'>
                                                <tr>
                                                    <td style='text-align:center; padding:30px 20px 10px 20px;'>
                                                        <img src='https://i.postimg.cc/pTRwypv7/TCG.png' alt='Logo TCB' width='150' height='70' style='margin-bottom:10px;' />
                                                        <h2 style='margin:0; color: #FFA500; font-size: 1.8rem;'>Redefinição de Senha</h2>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style='padding:10px 0; font-size: 1.2rem; color:#333;'>
                                                        Olá <strong>{usuario.NomeCompleto}</strong>,
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style='padding:10px 0; font-size: 1rem; color:#333;'>
                                                        Você solicitou a redefinição de senha.
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style='padding:20px 0; text-align:center;'>
                                                        <a href='{resetLink}' style='linear-gradient(90deg,#8A2BE2, #FFA500); color: white; padding:12px 20px; text-decoration:none; border-radius:5px; font-weight:bold; font-size: 1rem;'>Redefinir Senha</a>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style='padding-top:30px; font-size:13px; color:#888; text-align:center;'>
                                                        Se você não reconhece este e-mail, apenas ignore esta mensagem.
                                                    </td>
                                                </tr>
                                            </table>";

                    EnviarEmail(usuario.Email, "RedefinirSenha", corpoEmail,isHtml: true);

                    TempData["MensagemSucesso"] = "Email de recuperação enviado com sucesso!";
                    return RedirectToAction("Index", "Login");
                }
            }
            else
            {
                TempData["MensagemErro"] = "Email não encontrado!";
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpGet]
        public IActionResult AtualizarSenha(int id, string hash)
        {
            var usuario = db.Usuarios.FirstOrDefault(u => u.Id == id && u.Hash == hash);

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
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public IActionResult AtualizarSenha(AtualizarSenhaViewModel model)
        {
            var usuario = db.Usuarios.FirstOrDefault(u => u.Id == model.Id && u.Hash == model.Hash);

            if (usuario != null)
            {
                if (model.NovaSenha == model.ConfirmarSenha)
                {
                    usuario.Senha = Utilitarios.GerarHashSenha(model.NovaSenha, usuario.Salt);
                    usuario.Hash = Utilitarios.GeradorHash();
                    db.SaveChanges();

                    TempData["MensagemSucesso"] = "Senha atualizada com sucesso!";
                    return RedirectToAction("Index", "Login");
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