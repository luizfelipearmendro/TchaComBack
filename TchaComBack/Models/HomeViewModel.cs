namespace TchaComBack.Models
{
    public class HomeViewModel
    {
        public UsuariosModel Usuario { get; set; }

        public List<UsuariosModel> UsuarioNaoConfirmado { get; set; }
        public int TotalDeFuncionarios { get; set; }
        public int TotalDeSetores { get; set; }
        public int TotalDeUsuarios {  get; set; }
        public List<string> NomesSetores { get; set; }
        public List<int> QuantidadeFuncionariosAtivos { get; set; }
        public List<int> QuantidadeFuncionariosInativos { get; set; }

        public List<string> NomesCategorias { get; set; }
        public List<int> QuantidadeSetorAtivos { get; set; }
        public List<int> QuantidadeSetorInativos { get; set; }

        public List<string> NomesPerfis { get; set; }
        public List<int> QuantidadeUsuariosAtivosPorPerfil { get; set; }
        public List<int> QuantidadeUsuariosInativosPorPerfil { get; set; }

        public List<string> LabelsSexoFuncionarios { get; set; }
        public List<int> QuantidadeFuncionariosPorSexo { get; set; }

        public List<string> LabelsRankingSetores { get; set; }
        public List<int> QuantidadeFuncionariosRankingSetores { get; set; }

        public double PorcentagemAumentoFuncionarios { get; set; }

        public double PorcentagemAumentoSetores { get; set; }

        public double PorcentagemAumentoUsuarios { get; set; }
    }
}
