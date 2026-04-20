
using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Entities.Enum;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using ControleTarefas.Repositorio.Repositorios;
using Microsoft.EntityFrameworkCore;

namespace AgendamentoVacinacao.Repository.Repositories
{
    public class AgendamentoRepository : RepositorioBase<Agendamento>, IAgendamentoRepository
    {
        public AgendamentoRepository(Contexto contexto) : base(contexto) { }

        public async Task<List<AgendamentoDTO>> ConsultarAgendamentosPorPaciente(int pacienteId)
        {
            var agendamentos = await EntitySet.Where(a => a.idPaciente == pacienteId)
                                              .Include(a => a.paciente)
                                              .Select(a => MapToDTO(a))
                                              .ToListAsync();

            return agendamentos;
        }

        public async Task<List<AgendamentoDTO>> ListarAgendamentos()
        {
            var agendamentos = await EntitySet.Include(a => a.paciente)
                                              .Select(a => MapToDTO(a))
                                              .ToListAsync();

            return agendamentos;
        }

        public async Task<List<AgendamentoDTO>> ConsultarAgendamentosPorDia(DateOnly dia)
        {
            var agendamentos = await EntitySet.Where(a => a.dataAgendamento == dia)
                                              .Include(a => a.paciente)
                                              .Select(a => MapToDTO(a))
                                              .ToListAsync();

            return agendamentos;
        }

        public async Task<List<AgendamentoDTO>> ConsultarAgendamentosPorHorario(DateTime horario)
        {
            var data = DateOnly.FromDateTime(horario);
            var hora = TimeOnly.FromDateTime(horario);

            var agendamentos = await EntitySet.Where(a => a.dataAgendamento == data && a.horaAgendamento == hora)
                                              .Include(a => a.paciente)
                                              .Select(a => MapToDTO(a))
                                              .ToListAsync();

            return agendamentos;
        }

        public async Task<List<AgendamentoDTO>> FiltrarAgendamentos(StatusEnum status)
        {
            var agendamentos = await EntitySet.Where(a => a.status == status.ToString().ToLower())
                                              .Include(a => a.paciente)
                                              .Select(a => MapToDTO(a))
                                              .ToListAsync();

            return agendamentos;
        }

        public async Task<Agendamento> ObterAgendamentoPorId(int id)
        {
            var agendamento = await EntitySet.Include(a => a.paciente)
                                             .FirstOrDefaultAsync(a => a.Id == id);

            return agendamento;
        }

        private static AgendamentoDTO MapToDTO(Agendamento agendamento)
        {
            return new AgendamentoDTO
            {
                id = agendamento.Id,
                idPaciente = agendamento.idPaciente,
                dataAgendamento = agendamento.dataAgendamento,
                horaAgendamento = agendamento.horaAgendamento,
                status = agendamento.status,
                dataCriacao = agendamento.dataCriacao
            };
        }
    }
}
