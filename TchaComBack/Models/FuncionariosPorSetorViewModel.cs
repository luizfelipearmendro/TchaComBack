//namespace TchaComBack.Models
//{
//    public class FuncionariosPorSetorViewModel
//    {
//        public int SetorId { get; set; }
        
//        public string NomeSetor { get; set; }
        
//        public List<FuncionariosModel> Funcionarios { get; set; }

//        public int Quantidade { get; set; }
//    }
//}




namespace TchaComBack.Models
{
    public class FuncionariosPorSetorViewModel
    {
        public int SetorId { get; set; }

        public string NomeSetor { get; set; }

        public List<FuncionariosModel> Funcionarios { get; set; }

        public int QuantidadeFuncAtivos { get; set; }

        public int QuantidadeFuncInativos { get; set; }
    }
}
