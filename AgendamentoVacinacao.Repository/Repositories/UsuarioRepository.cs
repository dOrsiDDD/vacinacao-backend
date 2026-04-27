using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Entities.Filter;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using ControleTarefas.Repositorio.Repositorios;
using Microsoft.EntityFrameworkCore;

namespace AgendamentoVacinacao.Repository.Repositories
{
    public class UsuarioRepository : RepositorioBase<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(Contexto contexto) : base(contexto) { }

        public Task<Usuario> ObterUsuario(UsuarioFilter filtro)
        {
            var query = EntitySet.AsQueryable();

            if (filtro.id.HasValue)
                query = query.Where(e => e.Id == filtro.id.Value);

            if (!string.IsNullOrEmpty(filtro.email))
                query = query.Where(e => e.email == filtro.email);

            if (!string.IsNullOrEmpty(filtro.login))
                query = query.Where(e => e.login == filtro.login);

            return query.FirstOrDefaultAsync();
        }

        public Task<List<UsuarioDTO>> ListarTodos() 
        {
            var query = EntitySet.OrderBy(usuario => usuario.nome)
                            .Distinct()
                            .Select(usuario => new UsuarioDTO
                            {
                                nome = usuario.nome,
                                email = usuario.email,
                                dataCriacao = usuario.dataCriacao,
                                perfil = usuario.perfil
                            });

            return query.ToListAsync();
        }
    }
}
