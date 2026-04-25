using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Entities.Model;

namespace AgendamentoVacinacao.Business.Interface.IBusiness
{
    public interface IPacienteBusiness
    {
        // Métodos de Consulta Específicos
        Task<List<PacienteDTO>> ListarPacientes();
        Task<PacienteDTO> ObterPacientePorCPF(string cpf);
        Task<List<PacienteDTO>> ObterPacientesPorNome(string nome);
        Task<List<AgendamentoDTO>> ObterAgendamentosPorPaciente(string cpf);

        // Métodos do RepositorioBase
        Task<List<PacienteDTO>> Inserir(CadastroPacienteModel paciente);
        Task<PacienteDTO> AtualizarDataNascimento(string cpf, DateOnly novaDataNascimento);
        Task<PacienteDTO> AtualizarNome(string cpf, string nomeNovo);
        Task<List<PacienteDTO>> Deletar(string cpf);
    }
}
