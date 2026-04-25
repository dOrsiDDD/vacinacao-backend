using AgendamentoVacinacao.Entities.Enum;

namespace AgendamentoVacinacao.Entities.DTO
{
    public class AgendamentoDTO
    {
        public int id { get; set; }
        public int idPaciente { get; set; }
        public DateOnly dataAgendamento { get; set; }
        public TimeOnly horaAgendamento { get; set; }
        public StatusEnum status { get; set; }
        public DateTime dataCriacao { get; set; }
    }
}
