using TCBSistemaDeControle.Models;

namespace TCBSistemaDeControle.Repositories
{
    public interface IFuncionariosRepositorio
    {
        List<FuncionariosModel> BuscarTodosFuncionarios(int usuarioId);

        FuncionariosModel ListarPorId(int id);

        FuncionariosModel Cadastrar(FuncionariosModel contato);

        FuncionariosModel Editar(FuncionariosModel contato);

        //List<FuncionariosPorSetorViewModel> ObterFuncionariosPorSetor();

        bool Desativar(int id);

        bool Reativar(int id);
    }
}