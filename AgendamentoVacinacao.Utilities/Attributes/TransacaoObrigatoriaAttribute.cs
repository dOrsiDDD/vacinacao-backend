using System.Data;

namespace AgendamentoVacinacao.Utilitarios.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class TransacaoObrigatoriaAttribute : Attribute
    {
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadCommitted;

        public TransacaoObrigatoriaAttribute() { }

        public TransacaoObrigatoriaAttribute(IsolationLevel isolationLevel)
        {
            IsolationLevel = isolationLevel;
        }
    }
}

