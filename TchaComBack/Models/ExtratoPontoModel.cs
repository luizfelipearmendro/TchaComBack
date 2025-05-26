using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TchaComBack.Models
{
    public class ExtratoPontoModel
    {
        public int Id { get; set; }

        public int? Matricula { get; set; } 

        public DateTime DataBatida { get; set; }

        public TimeOnly? HoraEntrada1 { get; set; } //entrada manhã

        public TimeOnly? HoraSaida1 { get; set; } //saida manhã

        public TimeOnly? HoraEntrada2 { get; set; } //entrada tarde

        public TimeOnly? HoraSaida2 { get; set; }   //saída tarde

        public TimeOnly? HorasFaltas { get; set; }

        public TimeOnly? HorasExtras { get; set; }

        public string? Justificativa { get; set; } // justificativa para faltas ou atrasos

        [ForeignKey("Matricula")]
        [ValidateNever]
        public FuncionariosModel Funcionario { get; set; }
    }
}