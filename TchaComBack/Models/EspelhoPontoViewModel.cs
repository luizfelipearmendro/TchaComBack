namespace TchaComBack.Models
{
    public class EspelhoPontoViewModel
    {
        public int Mes { get; set; }
        public int Ano { get; set; }

        public List<ExtratoPontoModel> Pontos { get; set; } = new();
    }
}
