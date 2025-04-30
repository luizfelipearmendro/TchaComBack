namespace TchaComBack.Models
{
    public class RacaModel
    {
        public int Id { get; set; }

        public string Raca { get; set; }

        public ICollection<FuncionariosModel> Funcionarios { get; set; }
    }
}
