namespace TchaComBack.Models
{
    public class ConfiguracoesModel
    {
        public int Id { get; set; }

        public string? NomeConfiguracao { get; set; }

        public TimeOnly InicioExpediente { get; set; }

        public TimeOnly IntervaloInicio { get; set; }

        public TimeOnly IntervaloFim { get; set; }

        public TimeOnly FimExpediente { get; set; }

        public bool Ativo { get; set; }

        public int CriadoPor { get; set; }

        public DayOfWeek PrimeiroDiaExpediente { get; set; }
        
        public DayOfWeek UltimoDiaExpediente { get; set; }
    }
}
