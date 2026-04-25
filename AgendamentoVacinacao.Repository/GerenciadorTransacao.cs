using AgendamentoVacinacao.Repositorio.Interface;
using AgendamentoVacinacao.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AgendamentoVacinacao.Repositorio
{
    public class GerenciadorTransacao : IGerenciadorTransacao
    {
        private readonly Contexto _contexto;

        public GerenciadorTransacao(Contexto contexto)
        {
            _contexto = contexto;
        }

        public async Task BeginTransactionAsync(IsolationLevel isolationLevel)
        {
            var activeTransaction = _contexto.Database.CurrentTransaction;
            if (activeTransaction == null)
            {
                var connection = _contexto.Database.GetDbConnection();
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                var transaction = await connection.BeginTransactionAsync(isolationLevel);
                await _contexto.Database.UseTransactionAsync(transaction);
            }
        }

        public async Task CommitTransactionsAsync()
        {
            var contextHasChanges = _contexto.ChangeTracker.HasChanges();

            if (contextHasChanges)
                await _contexto.SaveChangesAsync();

            var activeTransaction = _contexto.Database.CurrentTransaction;
            await activeTransaction.CommitAsync();
            await activeTransaction.DisposeAsync();
        }

        public async Task RollbackTransactionsAsync()
        {
            var activeTransaction = _contexto.Database.CurrentTransaction;
            if (activeTransaction != null)
            {
                await activeTransaction.RollbackAsync();
                await activeTransaction.DisposeAsync();
            }
        }
    }
}
