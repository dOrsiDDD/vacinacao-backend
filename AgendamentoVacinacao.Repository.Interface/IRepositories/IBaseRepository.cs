using AgendamentoVacinacao.Entities.Entities;

namespace AgendamentoVacinacao.Repository.Interface.IRepositories
{
    public interface IBaseRepository<TEntidade> where TEntidade : class, IEntity
    {
        Task<TEntidade> ObterPorId(object id);
        Task<List<TEntidade>> Todos();
        Task<TEntidade> Inserir(TEntidade entidade);
        Task Inserir(IEnumerable<TEntidade> entidades);
        Task<TEntidade> Atualizar(TEntidade entidade);
        Task Deletar(TEntidade entidade);
        Task Deletar(IEnumerable<TEntidade> entidades);
        Task DeletarPorId(object id);
    }
}
