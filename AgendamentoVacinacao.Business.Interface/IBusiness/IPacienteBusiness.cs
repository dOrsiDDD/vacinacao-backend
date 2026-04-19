using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;

namespace AgendamentoVacinacao.Business.Interface.IBusiness
{
    public interface IPacienteBusiness
    {
        Task<List<PacienteDTO>> ListarPacientes();
        Task<Paciente> ObterPacientePorId(int id);
        Task<Paciente> ObterPacientePorNome(string nome);
    }
}
