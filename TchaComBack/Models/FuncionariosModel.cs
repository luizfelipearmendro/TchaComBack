using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TCBSistemaDeControle.Models
{
    public class FuncionariosModel
    {
        public int Id { get; set; } // id do funcionario

        public int UsuarioId { get; set; } // usuario que cadastrou o funcionario

        public string Nome { get; set; } // nome do funcionario

        public DateTime DataNascimento { get; set; } // data de nascimento do funcionario

        public char Sexo { get; set; } // sexo do funcionario

        public int Raca { get; set; } // raça do funcionario

        public int EstadoCivil { get; set; } // estado civil do funcionario

        public string? NomeMae { get; set; } // nome da mae do funcionario

        public string Naturalidade { get; set; } // naturalidade do funcionario

        public string Endereco { get; set; } // endereco do funcionario

        public string CidadeResidencia { get; set; } // cidade de residencia do funcionario

        [EmailAddress(ErrorMessage = "O e-mail informado não é válido!")]
        public string Email { get; set; } // e-mail do funcionario

        [Phone(ErrorMessage = "O celular informado não é válido!")]
        public string Celular { get; set; }// celular do funcionario

        public int SetorId { get; set; } // setor do funcionario

        public string Cargo { get; set; } // cargo do funcionario

        public Decimal Salario { get; set; } // salario do funcionario

        public DateTime DataIngresso { get; set; } // data de ingresso do funcionario

        public int DiasTrabalhados { get; set; } // dias trabalhados no mes

        public DateTime DataCadastro { get; set; } // data de cadastro do funcionario

        public DateTime? DataAtualizacao { get; set; } // data da ultima atualizacao do cadastro

        public char Ativo { get; set; } = 'S'; // status do funcionario




        public SetoresModel Setor { get; set; }

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
