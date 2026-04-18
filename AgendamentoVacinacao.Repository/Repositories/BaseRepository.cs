using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Repository;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace ControleTarefas.Repositorio.Repositorios
{
    public class RepositorioBase<TEntidade> : IBaseRepository<TEntidade> where TEntidade : class, IEntity
    {
        protected readonly Contexto _contexto;
        protected virtual DbSet<TEntidade> EntitySet { get; }

        public RepositorioBase(Contexto contexto)
        {
            _contexto = contexto;
            EntitySet = _contexto.Set<TEntidade>();
        }
        public async Task<TEntidade> Atualizar(TEntidade entidade)
        {
            var entityEntry = EntitySet.Update(entidade);

            await _contexto.SaveChangesAsync();

            return entityEntry.Entity;
        }

        public async Task Deletar(TEntidade entidade)
        {
            EntitySet.Remove(entidade);

            await _contexto.SaveChangesAsync();
        }

        public async Task Deletar(IEnumerable<TEntidade> entidades)
        {
            EntitySet.RemoveRange(entidades);

            await _contexto.SaveChangesAsync();
        }

        public async Task DeletarPorId(object id)
        {
            var entity = await EntitySet.FindAsync(id);

            if (entity != null)
                await Deletar(entity);
        }

        public async Task<TEntidade> Inserir(TEntidade entidade)
        {
            var entityEntry = await EntitySet.AddAsync(entidade);

            await _contexto.SaveChangesAsync();

            return entityEntry.Entity;
        }

        public async Task Inserir(IEnumerable<TEntidade> entidades)
        {
            await EntitySet.AddRangeAsync(entidades);

            await _contexto.SaveChangesAsync();
        }

        public async Task<TEntidade> ObterPorId(object id)
        {
            return await EntitySet.FindAsync(id);
        }

        public Task<List<TEntidade>> Todos() => EntitySet.ToListAsync();
    }
}
