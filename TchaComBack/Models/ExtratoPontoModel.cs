using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TchaComBack.Models
{
    public class ExtratoPontoModel
    {
        public int Id { get; set; }

        public int? Matricula { get; set; }

        public DateTime DataBatida { get; set; }

        // Todos os horários agora são inteiros representando minutos (por exemplo)
        public int? HoraEntrada1 { get; set; }
        public int? HoraSaida1 { get; set; }
        public int? HoraEntrada2 { get; set; }
        public int? HoraSaida2 { get; set; }
        public int? HorasTrabalhadas { get; set; }
        public int? HorasExtras { get; set; }
        public int? HorasNegativas { get; set; }
        public int? HorasPositivas { get; set; }
        public int? HorasACumprir { get; set; }

        [ForeignKey("Matricula")]
        [ValidateNever]
        public FuncionariosModel Funcionario { get; set; }
    }
}
