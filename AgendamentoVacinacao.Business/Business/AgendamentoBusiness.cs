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

        public async Task<Agendamento> Inserir(Agendamento agendamento)
        {
            _log.InfoFormat("Iniciando inserção de novo agendamento para paciente ID: {0}, Data: {1}, Hora: {2}", 
                agendamento.idPaciente, agendamento.dataAgendamento, agendamento.horaAgendamento);
            
            agendamento.dataCriacao = DateTime.Now;
            agendamento.status = agendamento.status ?? "pendente";
            
            var resultado = await _agendamentoRepository.Inserir(agendamento);
            
            _log.InfoFormat("Agendamento inserido com sucesso. ID: {0}", resultado.Id);
            
            return resultado;
        }

        public async Task Inserir(IEnumerable<Agendamento> agendamentos)
        {
            var agendamentosList = agendamentos.ToList();
            
            _log.InfoFormat("Iniciando inserção de {0} agendamentos em lote.", agendamentosList.Count);
            
            foreach (var agendamento in agendamentosList)
            {
                agendamento.dataCriacao = DateTime.Now;
                agendamento.status = agendamento.status ?? "pendente";
            }
            
            await _agendamentoRepository.Inserir(agendamentosList);
            
            _log.InfoFormat("{0} agendamentos inseridos com sucesso.", agendamentosList.Count);
        }

        public async Task<Agendamento> Atualizar(Agendamento agendamento)
        {
            _log.InfoFormat("Iniciando atualização do agendamento ID: {0}", agendamento.Id);
            
            var agendamentoExistente = await _agendamentoRepository.ObterAgendamentoPorId(agendamento.Id);
            
            if (agendamentoExistente == null)
            {
                _log.WarnFormat("Tentativa de atualizar agendamento inexistente. ID: {0}", agendamento.Id);
                throw new BusinessException($"Agendamento com ID {agendamento.Id} não encontrado.");
            }
            
            var resultado = await _agendamentoRepository.Atualizar(agendamento);
            
            _log.InfoFormat("Agendamento ID: {0} atualizado com sucesso.", agendamento.Id);
            
            return resultado;
        }

        public async Task Deletar(Agendamento agendamento)
        {
            _log.InfoFormat("Iniciando exclusão do agendamento ID: {0}", agendamento.Id);
            
            await _agendamentoRepository.Deletar(agendamento);
            
            _log.InfoFormat("Agendamento ID: {0} deletado com sucesso.", agendamento.Id);
        }

        public async Task Deletar(IEnumerable<Agendamento> agendamentos)
        {
            var agendamentosList = agendamentos.ToList();
            
            _log.InfoFormat("Iniciando exclusão de {0} agendamentos em lote.", agendamentosList.Count);
            
            await _agendamentoRepository.Deletar(agendamentosList);
            
            _log.InfoFormat("{0} agendamentos deletados com sucesso.", agendamentosList.Count);
        }

        public async Task DeletarPorId(int id)
        {
            if (id <= 0)
            {
                _log.WarnFormat("Tentativa de deletar agendamento com ID inválido: {0}", id);
                throw new BusinessException(string.Format(BusinessMessages.IdInvalido, id));
            }
            
            var agendamento = await _agendamentoRepository.ObterAgendamentoPorId(id);
            
            if (agendamento == null)
            {
                _log.WarnFormat("Tentativa de deletar agendamento inexistente. ID: {0}", id);
                throw new BusinessException(string.Format(BusinessMessages.IdInvalido, id));
            }
            
            _log.InfoFormat("Iniciando exclusão do agendamento ID: {0}", id);
            
            await _agendamentoRepository.DeletarPorId(id);
            
            _log.InfoFormat("Agendamento ID: {0} deletado com sucesso.", id);
        }

        public async Task<List<Agendamento>> Todos()
        {
            _log.InfoFormat("Obtendo lista completa de agendamentos.");
            
            var agendamentos = await _agendamentoRepository.Todos();
            
            _log.InfoFormat("Total de {0} agendamentos recuperados.", agendamentos.Count);
            
            return agendamentos;
        }

        public async Task ValidarAgendamento(DateOnly data, TimeOnly hora, int pacienteId)
        {
            if (data < DateOnly.FromDateTime(DateTime.Now))
                throw new BusinessException(string.Format(BusinessMessages.DataPassada));

            if (hora.Minute != 0)
            {
                _log.WarnFormat("Tentativa de agendamento em horário inválido: {0}", hora);
                throw new BusinessException(string.Format(BusinessMessages.HorarioInvalido, hora));
            }

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
