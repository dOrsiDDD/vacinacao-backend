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

        public async Task<Paciente> ObterPacientePorCPF(string cpf)
        {
            var paciente = await EntitySet.Include(p => p.agendamentos)
                                          .FirstOrDefaultAsync(p => p.cpf == cpf);

            return paciente;
        }

        public async Task<List<Paciente>> ObterPacientesPorNome(string nome)
        {
            var pacientes = await EntitySet.Include(p => p.agendamentos)
                                           .Where(p => p.nome.ToLower() == nome.ToLower())
                                           .ToListAsync();

            return pacientes;
        }

        public async Task<List<AgendamentoDTO>> ObterAgendamentosPorPaciente(string cpf)
        {
            var agendamentos = await EntitySet.Include(p => p.agendamentos)
                                              .Where(p => p.cpf == cpf)
                                              .SelectMany(p => p.agendamentos)
                                              .Select(a => new AgendamentoDTO
                                              {
                                                  idPaciente = a.idPaciente,
                                                  dataAgendamento = a.dataAgendamento,
                                                  horaAgendamento = a.horaAgendamento,
                                                  status = a.status,
                                                  dataCriacao = a.dataCriacao
                                              })
                                              .ToListAsync();

            return agendamentos;
        }

        public async Task<PacienteDTO> ConsultarPaciente(string cpf)
        {
            var paciente = await ObterPacientePorCPF(cpf);
            if (paciente == null)
            {
                return null;
            }
            return MapToDTO(paciente);
        }

        private static PacienteDTO MapToDTO(Paciente paciente)
        {
            return new PacienteDTO
            {
                id = paciente.Id,
                nome = paciente.nome,
                cpf = paciente.cpf,
                dataNascimento = paciente.dataNascimento,
                dataCriacao = paciente.dataCriacao,
                agendamentos = paciente.agendamentos
                    .Select(a => new AgendamentoDTO
                    {
                        id = a.Id,
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
