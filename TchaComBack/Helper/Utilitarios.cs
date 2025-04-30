using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace TCBSistemaDeControle.Helper
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
    }
}
