namespace TchaComBack.Models
{
    public class EstadoCivilModel
    {
        public int Id { get; set; }

        public string EstadoCivil { get; set; }

        public ICollection<FuncionariosModel> Funcionarios { get; set; }





        //ids tabela EstadoCivil
        //Id EstadoCivil
        //1	SOLTEIRO(A)
        //2	CASADO(A)
        //3	VIÚVO(A)
        //4	SEPARADO(A) JUDICIALMENTE
        //5	DIVORCIADO(A)
        //6	UNIÃO ESTÁVEL
        //7	SEPARADO DE FATO
    }
}
