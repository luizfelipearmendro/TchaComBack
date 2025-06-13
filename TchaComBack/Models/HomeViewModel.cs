namespace TchaComBack.Models
{
    public class HomeViewModel
    {

        public List<string> NomesSetores { get; set; } = new();
        public List<double> QuantidadeHorasFaltas { get; set; } = new();
        public List<double> QuantidadeHorasExtras { get; set; } = new();
        public List<string> LabelsHorasFaltasFormatadas { get; set; } = new();
        public List<string> LabelsHorasExtrasFormatadas { get; set; } = new();


        public List<string> LabelsMesesHorasFaltas { get; set; } = new();
        public List<double> QuantidadeHorasFaltasPorMes { get; set; } = new();
        public List<string> LabelsHorasFaltasPorMesFormatadas { get; set; } = new();


        public List<string> LabelsMesesHorasExtras { get; set; } = new();
        public List<double> QuantidadeHorasExtrasPorMes { get; set; } = new();
        public List<string> LabelsHorasExtrasPorMesFormatadas { get; set; } = new();



        public List<string> LabelsDiasSemana { get; set; } = new(); 
        public List<double> QuantidadeHorasFaltasPorDia { get; set; } = new();
        public List<string> LabelsHorasFaltasPorDiaFormatadas { get; set; } = new();


        public List<string> LabelsJustificativas { get; set; } = new();
        public List<int> QuantidadeOcorrenciasPorJustificativa { get; set; } = new();


        public double TotalHorasFaltas { get; set; }
        public string TotalHorasFaltasFormatado { get; set; } = "00:00";


        public double TotalHorasExtras { get; set; }
        public string TotalHorasExtrasFormatado { get; set; } = "00:00";

        public int TotalDiasTrabalhados { get; set; }
        public List<int> TotalDiasTrabalhadosPorMes { get; set; } = new();
        public List<string> LabelsDiasTrabalhadosPorMes { get; set; } = new();
    }
}
