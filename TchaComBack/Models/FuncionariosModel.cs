using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TchaComBack.Models
{
    public class FuncionariosModel
    {
        public int Id { get; set; }

        public int UsuarioResponsavelId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "O sexo é obrigatório.")]
        public char Sexo { get; set; }

        [Required(ErrorMessage = "A raça é obrigatória.")]
        [Range(1, 5, ErrorMessage = "Selecione uma raça válida.")]
        public int Raca { get; set; }

        [Required(ErrorMessage = "O estado civil é obrigatório.")]
        [Range(1, 8, ErrorMessage = "Selecione um estado civil válido.")]
        public int EstadoCivil { get; set; }

        public string? NomeMae { get; set; }

        [Required(ErrorMessage = "A naturalidade é obrigatória.")]
        public string Naturalidade { get; set; }

        [Required(ErrorMessage = "O endereço é obrigatório.")]
        public string Endereco { get; set; }

        [Required(ErrorMessage = "A cidade de residência é obrigatória.")]
        public string CidadeResidencia { get; set; }

        [EmailAddress(ErrorMessage = "O e-mail informado não é válido!")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "O celular informado não é válido!")]
        public string Celular { get; set; }

        public int SetorId { get; set; }

        [Required(ErrorMessage = "O cargo é obrigatório.")]
        public string Cargo { get; set; }

        [Required(ErrorMessage = "O salário é obrigatório.")]
        [Range(0, double.MaxValue, ErrorMessage = "O salário deve ser maior ou igual a zero.")]
        public decimal Salario { get; set; }

        public DateTime DataIngresso { get; set; } // Pode adicionar Required se for obrigatório


        public int DiasTrabalhados { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public char Ativo { get; set; } = 'S';

        public string TipoContrato { get; set; }

        [Required]
        public int? Matricula { get; set; }


        // crud
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


        // Relacionamentos

        [InverseProperty("Funcionario")]
        public ICollection<UsuariosModel>? UsuariosVinculados { get; set; }

        [ValidateNever]
        public ICollection<ExtratoPontoModel> ExtratosDePonto { get; set; } = new List<ExtratoPontoModel>();

        [ValidateNever]
        public SetoresModel Setor { get; set; }

        [ValidateNever]
        public RacaModel RacaNav { get; set; }

        [ValidateNever]
        public EstadoCivilModel EstadoCivilNav { get; set; }

        [ValidateNever]
        public UsuariosModel UsuarioResponsavel { get; set; }
    }
}