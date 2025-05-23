using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TchaComBack.Models
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

        public int? SetorId { get; set; }

        public char Ativo { get; set; } = 'S';

        public DateTime UltimoAcesso { get; set; }

        [ValidateNever]
        public SetoresModel Setor { get; set; }

        [StringLength(6)]
        public string? Matricula { get; set; } // Agora é anulável

        [ForeignKey("Matricula")]
        [ValidateNever]
        public FuncionariosModel? Funcionario { get; set; } // Relacionamento opcional

        public void Desativar()
        {
            this.Ativo = 'N';
        }

        public void Reativar()
        {
            this.Ativo = 'S';
        }

        public bool EstaAtivo()
        {
            return this.Ativo == 'S';
        }
    }
}