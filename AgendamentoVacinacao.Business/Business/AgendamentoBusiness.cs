using AgendamentoVacinacao.Business.Interface.IBusiness;
using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Entities.Enum;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Utilities.Exceptions;
using AgendamentoVacinacao.Utilities.Messages;
using AgendamentoVacinacao.Utilities.Constants;
using log4net;

namespace AgendamentoVacinacao.Business.Business
{
    public class AgendamentoBusiness : IAgendamentoBusiness
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AgendamentoBusiness));
        private readonly IAgendamentoRepository _agendamentoRepository;

        public AgendamentoBusiness(IAgendamentoRepository agendamentoRepository)
        {
            _agendamentoRepository = agendamentoRepository;
        }

        public async Task<List<AgendamentoDTO>> ConsultarAgendamentosPorPaciente(int pacienteId)
        {
            if (pacienteId <= 0)
            {
                _log.WarnFormat("Tentativa de consultar agendamentos com ID de paciente inválido: {0}", pacienteId);
                throw new BusinessException(string.Format(BusinessMessages.IdInvalido, pacienteId));
            }

            return await _agendamentoRepository.ConsultarAgendamentosPorPaciente(pacienteId);
        }

        public async Task<List<AgendamentoDTO>> ListarAgendamentos()
        {
            return await _agendamentoRepository.ListarAgendamentos();
        }

        public async Task<List<AgendamentoDTO>> ConsultarAgendamentosPorDia(DateOnly dia)
        {
            var agendamentos = await _agendamentoRepository.ConsultarAgendamentosPorDia(dia);

            if (agendamentos == null || agendamentos.Count == 0)
            {
                _log.InfoFormat("Nenhum agendamento encontrado para o dia: {0}", dia);
            }

            return agendamentos;
        }

        public async Task<List<AgendamentoDTO>> ConsultarAgendamentosPorHorario(DateTime horario)
        {
            return await _agendamentoRepository.ConsultarAgendamentosPorHorario(horario);
        }

        public async Task<List<AgendamentoDTO>> FiltrarAgendamentos(StatusEnum status)
        {
            return await _agendamentoRepository.FiltrarAgendamentos(status);
        }

        public async Task<Agendamento> ObterAgendamentoPorId(int id)
        {
            if (id <= 0)
            {
                _log.WarnFormat("Tentativa de obter agendamento com ID inválido: {0}", id);
                throw new BusinessException(string.Format(BusinessMessages.IdInvalido, id));
            }

            var agendamento = await _agendamentoRepository.ObterAgendamentoPorId(id);

            if (agendamento == null)
            {
                _log.InfoFormat("Agendamento com ID {0} não encontrado.", id);
                throw new BusinessException(string.Format(BusinessMessages.IdInvalido, id));
            }

            return agendamento;
        }

        public async Task ValidarAgendamento(DateOnly data, TimeOnly hora, int pacienteId)
        {
            if (hora.Minute != 0)
            {
                _log.WarnFormat("Tentativa de agendamento em horário inválido: {0}", hora);
                throw new BusinessException(string.Format(BusinessMessages.HorarioInvalido, hora));
            }

            if (data < DateOnly.FromDateTime(DateTime.Now))
                throw new BusinessException(string.Format(BusinessMessages.DataPassada));

            var agendamentosDoDia = await _agendamentoRepository.ConsultarAgendamentosPorDia(data);

            if (agendamentosDoDia.Count >= BusinessConstants.MAX_AGENDAMENTOS_POR_DIA)
            {
                _log.WarnFormat("Tentativa de agendamento excedendo limite diário. Data: {0}, Agendamentos: {1}", data, agendamentosDoDia.Count);
                throw new BusinessException(string.Format(BusinessMessages.DiaEsgotado, BusinessConstants.MAX_AGENDAMENTOS_POR_DIA));
            }

            var agendamentosNoHorario = agendamentosDoDia
                .Where(a => a.horaAgendamento == hora)
                .ToList();

            if (agendamentosNoHorario.Count >= BusinessConstants.MAX_AGENDAMENTOS_POR_HORARIO)
            {
                _log.WarnFormat("Tentativa de agendamento excedendo limite horário. Data: {0}, Hora: {1}, Agendamentos: {2}", data, hora, agendamentosNoHorario.Count);
                throw new BusinessException(string.Format(BusinessMessages.HorarioEsgotado, BusinessConstants.MAX_AGENDAMENTOS_POR_HORARIO));
            }
        }
    }
}
