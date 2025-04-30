namespace TCBSistemaDeControle.Models
{
    public class FuncionariosPorSetorViewModel
    {
        public int SetorId { get; set; }
        
        public string NomeSetor { get; set; }
        
        public List<FuncionariosModel> Funcionarios { get; set; }

        public int Quantidade { get; set; }
    }
}
