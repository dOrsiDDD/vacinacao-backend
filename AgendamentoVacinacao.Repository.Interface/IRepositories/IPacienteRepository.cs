using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;

namespace AgendamentoVacinacao.Repository.Interface.IRepositories
{
    public interface IPacienteRepository : IBaseRepository<Paciente>
    {
        Task<List<PacienteDTO>> ListarPacientes();
        Task<Paciente> ObterPacientePorId(int id);
        Task<Paciente> ObterPacientePorNome(string nome);
        Task<List<AgendamentoDTO>> ObterAgendamentosPorPaciente(int pacienteId);
        Task<PacienteDTO> ConsultarPaciente(int id);
    }
}
