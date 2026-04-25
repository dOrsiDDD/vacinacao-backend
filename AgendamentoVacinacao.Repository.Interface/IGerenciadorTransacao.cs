using System.Data;

namespace AgendamentoVacinacao.Repositorio.Interface
{
    public interface IGerenciadorTransacao
    {
        Task BeginTransactionAsync(IsolationLevel isolationLevel);
        Task CommitTransactionsAsync();
        Task RollbackTransactionsAsync();
    }
}