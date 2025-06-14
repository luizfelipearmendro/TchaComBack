﻿namespace TchaComBack.Models
{
    public class UsuariosViewModel
    {
        public int QtdUsuariosAtivos { get; set; }

        public int QtdUsuariosInativos { get; set; }

        public IEnumerable<UsuariosModel> Usuarios { get; set; }

        public int PaginaAtual { get; set; }

        public int TotalPaginas { get; set; }
    }
}