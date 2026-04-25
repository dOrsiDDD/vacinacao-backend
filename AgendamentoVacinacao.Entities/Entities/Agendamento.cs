using AgendamentoVacinacao.Entities.Enum;

namespace AgendamentoVacinacao.Entities.Entities
{
    public class Agendamento : IdEntity<int>
    {
        public int idPaciente { get; set; }
        public DateOnly dataAgendamento { get; set; }
        public TimeOnly horaAgendamento { get; set; }
        public StatusEnum status { get; set; }
        public DateTime dataCriacao { get; set; }

        public Paciente paciente { get; set; }

        public Agendamento() { }
    }
}
