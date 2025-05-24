using System.ComponentModel.DataAnnotations.Schema;

namespace TchaComBack.Models
{
    public class ExtratoPontoModel
    {
        public int Id { get; set; }

        public int? Matricula { get; set; } 

        public DateTime DataBatida { get; set; }

        public TimeSpan? HoraEntrada1 { get; set; }

        public TimeSpan? HoraSaida1 { get; set; }

        public TimeSpan? HoraEntrada2 { get; set; }

        public TimeSpan? HoraSaida2 { get; set; }

        [ForeignKey("Matricula")]
        [ValidateNever]
        public FuncionariosModel Funcionario { get; set; }
    }
}