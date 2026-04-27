using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Entities.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoVacinacao.Repository.Interface.IRepositories
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<List<UsuarioDTO>> ListarTodos();
        Task<Usuario> ObterUsuario(UsuarioFilter filtro);
    }
}
