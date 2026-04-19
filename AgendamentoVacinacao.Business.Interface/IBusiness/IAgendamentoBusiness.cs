using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Entities.Enum;

namespace AgendamentoVacinacao.Business.Interface.IBusiness
{
    public interface IAgendamentoBusiness
    {
        Task<List<AgendamentoDTO>> ConsultarAgendamentosPorPaciente(int pacienteId);
        Task<List<AgendamentoDTO>> ListarAgendamentos();
        Task<List<AgendamentoDTO>> ConsultarAgendamentosPorDia(DateOnly dia);
        Task<List<AgendamentoDTO>> ConsultarAgendamentosPorHorario(DateTime horario);
        Task<List<AgendamentoDTO>> FiltrarAgendamentos(StatusEnum status);
        Task<Agendamento> ObterAgendamentoPorId(int id);
    }
}
