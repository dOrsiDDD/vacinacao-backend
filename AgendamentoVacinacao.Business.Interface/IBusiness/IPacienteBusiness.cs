using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Entities.Model;

namespace AgendamentoVacinacao.Business.Interface.IBusiness
{
    public interface IPacienteBusiness
    {
        // Métodos de Consulta Específicos
        Task<List<PacienteDTO>> ListarPacientes();
        Task<PacienteDTO> ObterPacientePorId(int id);
        Task<PacienteDTO> ObterPacientePorNome(string nome);
        Task<List<AgendamentoDTO>> ObterAgendamentosPorPaciente(int pacienteId);

        // Métodos do RepositorioBase
        Task<List<PacienteDTO>> Inserir(CadastroPacienteModel paciente);
        Task<PacienteDTO> AtualizarDataNascimento(string nome, DateOnly novaDataNascimento);
        Task<PacienteDTO> AtualizarNome(string nomeAntigo, string nomeNovo);
        Task<List<PacienteDTO>> Deletar(int pacienteId);
    }
}
