namespace TchaComBack.Models
{
    public class ConfiguracoesModel
    {
        public int Id { get; set; }

        public int IdUsuario { get; set; }

        public TimeOnly InicioExpediente { get; set; }

        public TimeOnly FimExpediente { get; set; }
    }
}
