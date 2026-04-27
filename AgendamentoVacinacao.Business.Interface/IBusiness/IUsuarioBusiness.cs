using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Filter;
using AgendamentoVacinacao.Entities.Model;

namespace AgendamentoVacinacao.Business.Interface.IBusiness
{
    public interface IUsuarioBusiness
    {
        Task<UsuarioDTO> InserirUsuario(CadastroUsuarioModel novoUsuario);
        Task<List<UsuarioDTO>> ListarTodos();
        Task<UsuarioDTO> ConsultarUsuario(UsuarioFilter filtro);
    }
}
