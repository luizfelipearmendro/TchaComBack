using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TchaComBack.Models
{
    public class ExtratoPontoModel
    {
        public int Id { get; set; }

        public DateTime DataBatida { get; set; }

        public TimeSpan? HoraEntrada1 { get; set; }

        public TimeSpan? HoraSaida1 { get; set; }

        public TimeSpan? HoraEntrada2 { get; set; }

        public TimeSpan? HoraSaida2 { get; set; }

        public int FuncionarioId { get; set; }
        [ForeignKey("FuncionarioId")]
        public FuncionariosModel Funcionario { get; set; }
    }
}
