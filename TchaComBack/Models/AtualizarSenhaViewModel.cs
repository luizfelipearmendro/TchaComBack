using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCBSistemaDeControle.Models
{
    public class AtualizarSenhaViewModel
    {
        [NotMapped]
        public int Id { get; set; }
        
        [NotMapped]
        public string Hash { get; set; }
        
        [NotMapped]
        public string NovaSenha { get; set; }
        
        [NotMapped]
        public string ConfirmarSenha { get; set; }
    }
}
