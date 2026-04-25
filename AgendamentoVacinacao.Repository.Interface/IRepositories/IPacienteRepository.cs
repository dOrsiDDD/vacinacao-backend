using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;

namespace AgendamentoVacinacao.Repository.Interface.IRepositories
{
    public interface IPacienteRepository : IBaseRepository<Paciente>
    {
        Task<List<PacienteDTO>> ListarPacientes();
        Task<Paciente> ObterPacientePorCPF(string cpf);
        Task<List<Paciente>> ObterPacientesPorNome(string nome);
        Task<List<AgendamentoDTO>> ObterAgendamentosPorPaciente(string cpf);
        Task<PacienteDTO> ConsultarPaciente(string cpf);
    }
}
