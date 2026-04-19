using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Entities.Enum;

namespace AgendamentoVacinacao.Business.Interface.IBusiness
{
    public interface IAgendamentoBusiness
    {
        // Métodos de Consulta Específicos
        Task<List<AgendamentoDTO>> ConsultarAgendamentosPorPaciente(int pacienteId);
        Task<List<AgendamentoDTO>> ListarAgendamentos();
        Task<List<AgendamentoDTO>> ConsultarAgendamentosPorDia(DateOnly dia);
        Task<List<AgendamentoDTO>> ConsultarAgendamentosPorHorario(DateTime horario);
        Task<List<AgendamentoDTO>> FiltrarAgendamentos(StatusEnum status);
        Task<Agendamento> ObterAgendamentoPorId(int id);

        // Métodos do RepositorioBase
        Task<Agendamento> Inserir(Agendamento agendamento);
        Task Inserir(IEnumerable<Agendamento> agendamentos);
        Task<Agendamento> Atualizar(Agendamento agendamento);
        Task Deletar(Agendamento agendamento);
        Task Deletar(IEnumerable<Agendamento> agendamentos);
        Task DeletarPorId(int id);
        Task<List<Agendamento>> Todos();

        // Validações de Negócio
        Task ValidarAgendamento(DateOnly data, TimeOnly hora, int pacienteId);
    }
}
