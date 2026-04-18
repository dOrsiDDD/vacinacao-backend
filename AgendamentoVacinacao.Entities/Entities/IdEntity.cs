namespace AgendamentoVacinacao.Entities.Entities
{
    public abstract class IdEntity<T> : IEntity
    {
        T Id { get; set; }
    }
}
