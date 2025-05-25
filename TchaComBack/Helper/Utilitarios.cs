using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace TchaComBack.Helper
{
    public class Utilitarios
    {
        public static string GeradorHash(int length = 30)
        {
            char[] CaracteresDisponiveis = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

            Random random = new Random();

            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = CaracteresDisponiveis[random.Next(CaracteresDisponiveis.Length)];
            }
            return new string(result);
        }

        public static decimal ConverteMoeda(decimal moeda)
        {
            return moeda;
        }

        public static string GerarHashSenha(string senha, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var senhaSalt = senha + salt;
                var bytes = Encoding.UTF8.GetBytes(senhaSalt);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static string GerarSalt()
        {
            var random = new RNGCryptoServiceProvider();
            var salt = new byte[16];
            random.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        public static string PrimeiraLetraMaiuscula(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return texto;

            var palavras = texto.ToLower().Split(' ');
            for (int i = 0; i < palavras.Length; i++)
            {
                if (palavras[i].Length > 0)
                {
                    palavras[i] = char.ToUpper(palavras[i][0]) + palavras[i][1..];
                }
            }
            return string.Join(" ", palavras);
        }

        public static bool SenhaEhForte(string senha, out string mensagemErro)
        {
            mensagemErro = "";

            if (string.IsNullOrWhiteSpace(senha) || senha.Length < 8)
            {
                mensagemErro = "A senha deve ter no mínimo 8 caracteres.";
                return false;
            }
            if (!senha.Any(char.IsUpper))
            {
                mensagemErro = "A senha deve conter ao menos uma letra maiúscula.";
                return false;
            }
            if (!senha.Any(char.IsLower))
            {
                mensagemErro = "A senha deve conter ao menos uma letra minúscula.";
                return false;
            }
            if (!senha.Any(char.IsDigit))
            {
                mensagemErro = "A senha deve conter ao menos um número.";
                return false;
            }
            if (!senha.Any(ch => "!@#$%^&*()_+-=[]{}|;:',.<>?/`~".Contains(ch)))
            {
                mensagemErro = "A senha deve conter ao menos um caractere especial.";
                return false;
            }

            return true;
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
    }
}
