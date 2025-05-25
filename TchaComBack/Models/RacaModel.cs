namespace TchaComBack.Models
{
    public class RacaModel
    {
        public int Id { get; set; }

        public string Raca { get; set; }

        public ICollection<FuncionariosModel> Funcionarios { get; set; }


        //ids tabela Raca
        //Id Raca
        //1	PRETO
        //2	BRANCO
        //3	PARDO
        //4	AMARELO
        //5	INDÍGENA
    }
}
