namespace AgendamentoVacinacao.Entities.Entities
{
    public abstract class IdEntity<T> : IEntity
    {
        public T Id { get; set; }
    }
}
