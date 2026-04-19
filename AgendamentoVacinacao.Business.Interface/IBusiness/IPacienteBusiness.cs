using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;

namespace AgendamentoVacinacao.Business.Interface.IBusiness
{
    public interface IPacienteBusiness
    {
        // Métodos de Consulta Específicos
        Task<List<PacienteDTO>> ListarPacientes();
        Task<Paciente> ObterPacientePorId(int id);
        Task<Paciente> ObterPacientePorNome(string nome);

        // Métodos do RepositorioBase
        Task<Paciente> Inserir(Paciente paciente);
        Task Inserir(IEnumerable<Paciente> pacientes);
        Task<Paciente> Atualizar(Paciente paciente);
        Task Deletar(Paciente paciente);
        Task Deletar(IEnumerable<Paciente> pacientes);
        Task DeletarPorId(int id);
        Task<List<Paciente>> Todos();
    }
}
