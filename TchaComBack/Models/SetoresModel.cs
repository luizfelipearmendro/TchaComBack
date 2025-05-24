using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TchaComBack.Models
{
    public class SetoresModel
    {
        
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do setor é obrigatório.")]
        public string Nome { get; set; }

        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome do responsável é obrigatório.")]
        public string ResponsavelSetor { get; set; }

        [Required(ErrorMessage = "O e-mail do responsável é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é válido!")]
        public string EmailResponsavelSetor { get; set; }

        [Required]
        public char SexoResponsavel { get; set; }

        [Required(ErrorMessage = "A localização do setor é obrigatória.")]
        public string Localizacao { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime DataAtualizacao { get; set; }

        public char Ativo { get; set; } = 'S';

        public int UsuarioResponsavelId  { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A categoria do setor deve ser selecionada.")]
        public int CategoriaId { get; set; }

        public string? ImagemSetor { get; set; }





        [ValidateNever]
        public CategoriaModel Categoria { get; set; }

        [ValidateNever]
        public ICollection<FuncionariosModel> Funcionarios { get; set; }

        [ValidateNever]
        public ICollection<UsuariosModel> Usuarios { get; set; }


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