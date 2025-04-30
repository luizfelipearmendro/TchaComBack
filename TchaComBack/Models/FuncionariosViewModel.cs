namespace TchaComBack.Models
{
    public class FuncionariosViewModel
    {
        public string NomeSetor { get; set; }

        public List<FuncionariosModel> Funcionarios { get; set; }

        public int QuantidadeFuncAtivos { get; set; }

        public int QuantidadeFuncInativos { get; set; }
    }
}