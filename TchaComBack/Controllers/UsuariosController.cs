using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using TCBSistemaDeControle.Data;
using TCBSistemaDeControle.Helper;
using TCBSistemaDeControle.Models;

namespace TCBSistemaDeControle.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext db;

        public UsuariosController(ApplicationDbContext _db)
        {
            db = _db;
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
            if(validaEmailExistente == true)
            {
                TempData["MensagemErro"] = "Ops, o e-mail informado já existe!";
                return RedirectToAction("Index", "Login");
            }

            var salt = Utilitarios.GerarSalt();
            usuario.Salt = salt;
            usuario.Senha = Utilitarios.GerarHashSenha(usuario.Senha, salt);
            usuario.Hash = Utilitarios.GeradorHash();
            usuario.Confirmado = 0;
            usuario.DataCadastro = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Usuarios.Add(usuario);
                db.SaveChanges();

                TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso!";
                return RedirectToAction("Index", "Login");
            }
            return RedirectToAction("Index", "Login");
        }

        public void EnviarEmail(string para, string assunto, string mensagemCorpo)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("TCBSistemaDeControle", "tchacomback@gmail.com"));
            message.To.Add(new MailboxAddress("tchacomback@gmail.com", para));
            message.Subject = assunto;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = @"
                <html>
                    <body>
                        <img src='cid:logo' alt='Logo TCB' width='70' height='60' />
                        <p>" + mensagemCorpo + @"</p>
                    </body>
                </html>"
            };

            // Caminho físico da imagem
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "logo2TCB-removebg-preview.png");

            // Adicionando a logo como anexo inline
            bodyBuilder.Attachments.Add(logoPath, new ContentType("image", "png") { Name = "logo2TCB-removebg-preview.png" })
                         .ContentId = "logo";

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    //Utilizar mailtrap.io ou mailgun
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
                    client.Dispose();
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

                    EnviarEmail(usuario.Email, "Recuperação de Senha", $"Olá {usuario.NomeCompleto}, você solicitou a recuperação de sua senha, clique para <a href='{resetLink}'>Redefinir sua Senha</a>");

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
