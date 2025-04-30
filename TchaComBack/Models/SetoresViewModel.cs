    namespace TCBSistemaDeControle.Models
    {
        public class SetoresViewModel
        {
            public int SetorId { get; set; }

            public int Quantidade { get; set; }

            public IEnumerable<SetoresViewModel> QuantidadePorSetor { get; set; }

            public IEnumerable<SetoresModel> Setores { get; set; }

            public IEnumerable<CategoriaModel> Categorias { get; set; }
        }
    }
