using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCBSistemaDeControle.Models
{
    public class SetoresModel
    {
        
        public int Id { get; set; } // Identificador único  
        
        public string Nome { get; set; } = string.Empty; // Nome do setor  
        
        public string Descricao { get; set; } = string.Empty; // Descrição do setor  
        
        public string ResponsavelSetor { get; set; } = string.Empty; // Nome do responsável pelo setor  

        [EmailAddress(ErrorMessage = "O e-mail informado não é válido!")]
        public string EmailResponsavelSetor { get; set; } = string.Empty; // E-mail do responsável pelo setor

        public char SexoResponsavel { get; set; } //Sexo do responsável pelo setor

        public string Localizacao { get; set; } = string.Empty; // Localização dentro da empresa  
        
        public DateTime DataCriacao { get; set; } // Data de criação do setor  

        public DateTime DataAtualizacao { get; set; } // Data de atualização do setor

        public char Ativo { get; set; } = 'S';  // Indica se o setor está ativo ou não

        public int UsuarioId { get; set; } // Identificador do usuário que criou o setor

        public int CategoriaId { get; set; }  // Identificador da categoria daquele setor

        public string? ImagemSetor { get; set; } // Imagem capa Setor



        //public virtual ICollection<FuncionariosModel> Funcionarios { get; set; } // conecta o relacionamento com a Setores

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