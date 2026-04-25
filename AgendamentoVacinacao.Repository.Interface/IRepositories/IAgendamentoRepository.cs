using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Entities.Enum;

namespace AgendamentoVacinacao.Repository.Interface.IRepositories
{
    public interface IAgendamentoRepository : IBaseRepository<Agendamento>
    {
        Task<List<AgendamentoDTO>> ConsultarAgendamentosPorPaciente(int pacienteId);
        Task<List<AgendamentoDTO>> ListarAgendamentos();
        Task<List<AgendamentoDTO>> ConsultarAgendamentosPorDia(DateOnly dia);
        Task<List<AgendamentoDTO>> ConsultarAgendamentosPorHorario(DateTime horario);
        Task<List<AgendamentoDTO>> FiltrarAgendamentos(StatusEnum status);
        Task<AgendamentoDTO> ConsultarAgendamentoPorId(int id);
        Task<Agendamento> ObterAgendamentoPorId(int id);   
    }
}
