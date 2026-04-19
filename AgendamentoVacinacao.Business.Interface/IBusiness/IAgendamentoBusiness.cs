using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Entities.Enum;
using AgendamentoVacinacao.Entities.Model;

namespace AgendamentoVacinacao.Business.Interface.IBusiness
{
    public interface IAgendamentoBusiness
    {
        // Métodos de Consulta Específicos
        Task<List<AgendamentoDTO>> ListarAgendamentos();
        Task<List<AgendamentoDTO>> ConsultarAgendamentosPorDia(DateOnly dia);
        Task<List<AgendamentoDTO>> ConsultarAgendamentosPorHorario(DateTime horario);
        Task<List<AgendamentoDTO>> FiltrarAgendamentos(StatusEnum status);
        Task<Agendamento> ObterAgendamentoPorId(int id);

        // Métodos do RepositorioBase
        Task<List<AgendamentoDTO>> Inserir(CadastroAgendamentoModel agendamento);
        Task<List<AgendamentoDTO>> AtualizarData(int id, DateTime novaData);
        Task<List<AgendamentoDTO>> AtualizarStatus(int id, StatusEnum novoStatus);
        Task Deletar(IEnumerable<int> agendamentosIds);
        Task Deletar(int id);

        // Validações de Negócio
        Task ValidarAgendamento(DateOnly data, TimeOnly hora, int pacienteId);
    }
}
