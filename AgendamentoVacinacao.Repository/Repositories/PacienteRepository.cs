using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using ControleTarefas.Repositorio.Repositorios;
using Microsoft.EntityFrameworkCore;

namespace AgendamentoVacinacao.Repository.Repositories
{
    public class PacienteRepository : RepositorioBase<Paciente>, IPacienteRepository
    {
        public PacienteRepository(Contexto contexto) : base(contexto) { }

        public async Task<List<PacienteDTO>> ListarPacientes()
        {
            var pacientes = await EntitySet.Include(p => p.agendamentos)
                                           .Select(p => MapToDTO(p))
                                           .ToListAsync();

            return pacientes;
        }

        public async Task<Paciente> ObterPacientePorId(int id)
        {
            var paciente = await EntitySet.Include(p => p.agendamentos)
                                          .FirstOrDefaultAsync(p => p.Id == id);

            return paciente;
        }

        public async Task<Paciente> ObterPacientePorNome(string nome)
        {
            var paciente = await EntitySet.Include(p => p.agendamentos)
                                          .FirstOrDefaultAsync(p => p.nome == nome);

            return paciente;
        }

        private static PacienteDTO MapToDTO(Paciente paciente)
        {
            return new PacienteDTO
            {
                nome = paciente.nome,
                dataNascimento = paciente.dataNascimento,
                dataCriacao = paciente.dataCriacao,
                agendamentos = paciente.agendamentos
                    .Select(a => new AgendamentoDTO
                    {
                        idPaciente = a.idPaciente,
                        dataAgendamento = a.dataAgendamento,
                        horaAgendamento = a.horaAgendamento,
                        status = a.status,
                        dataCriacao = a.dataCriacao
                    })
                    .ToList()
            };
        }
    }
}
