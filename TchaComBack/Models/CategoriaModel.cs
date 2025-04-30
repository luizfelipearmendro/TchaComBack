using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace TchaComBack.Models
{
    public class CategoriaModel
    {
        public int Id { get; set; } 

        public string Nome { get; set; } // Nome da categoria

        public string Descricao { get; set; } = string.Empty; // Descrição da categoria

        public DateTime DataCriacao { get; set; } = DateTime.Now; // Data de criação

        public DateTime? DataAtualizacao { get; set; } // Data de atualização 


        [ValidateNever]
        public ICollection<SetoresModel> Setores { get; set; }

    }
}