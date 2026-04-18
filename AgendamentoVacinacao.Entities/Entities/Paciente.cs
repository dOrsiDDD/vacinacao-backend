namespace AgendamentoVacinacao.Entities.Entities
{
    public class Paciente : IdEntity<int>
    {
        public string nome { get; set; }
        public DateOnly dataNascimento { get; set; }
        public DateTime dataCriacao { get; set; }

        public ICollection<Agendamento> agendamentos { get; set; } = new List<Agendamento>();

        public Paciente() { }
    }
}
