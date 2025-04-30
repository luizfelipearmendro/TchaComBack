using System.ComponentModel.DataAnnotations;

namespace TCBSistemaDeControle.Models
{
    public class UsuariosModel
    {
        public int Id { get; set; }

        [EmailAddress(ErrorMessage = "O e-mail informado não é válido!")]
        public string Email { get; set; }

        public string Senha { get; set; }

        public string? NomeCompleto { get; set; }

        public int? TipoPerfil { get; set; }

        public string? Hash { get; set; }

        public int? Confirmado { get; set; }

        public string? Salt { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime DataHoraEsqueceuSenha { get; set; }
    }
}
