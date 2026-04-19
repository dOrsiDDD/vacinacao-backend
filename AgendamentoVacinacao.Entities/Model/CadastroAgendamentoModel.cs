namespace AgendamentoVacinacao.Entities.Model
{
    public class CadastroAgendamentoModel
    {
        public int idPaciente { get; set; }
        public DateOnly dataAgendamento { get; set; }
        public TimeOnly horaAgendamento { get; set; }
    }
}
