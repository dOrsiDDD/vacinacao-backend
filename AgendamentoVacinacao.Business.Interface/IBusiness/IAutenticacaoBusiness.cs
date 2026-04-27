using AgendamentoVacinacao.Entities.DTO;

namespace AgendamentoVacinacao.Business.Interface.IBusiness
{
    public interface IAutenticacaoNegocio
    {
        Task<TokenUsuarioDTO> Login(string login, string senha);
        Task<TokenUsuarioDTO> RefreshToken();
    }
}
