﻿using TchaComBack.Models;

namespace TchaComBack.Repositories
{
    public interface ISetoresRepositorio
    {
        List<SetoresModel> BuscarTodosSetores(int UsuarioResponsavelId );

        SetoresModel ListarPorId(int Id);

        SetoresModel Cadastrar(SetoresModel setor);

        SetoresModel Editar(SetoresModel setor);

        bool Desativar(int id);

        bool Reativar(int id);
    }
}