using AgendamentoVacinacao.Business.Interface.IBusiness;
using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Entities.Enum;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Utilities.Exceptions;
using AgendamentoVacinacao.Utilities.Messages;
using AgendamentoVacinacao.Utilities.Constants;
using log4net;
using AgendamentoVacinacao.Entities.Model;
using System.Drawing;
using System.Runtime.ExceptionServices;

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

        public async Task<AgendamentoDTO> Inserir(CadastroAgendamentoModel agendamento)
        {
            _log.InfoFormat("Iniciando inserção de novo agendamento para paciente ID: {0}, Data: {1}, Hora: {2}", agendamento.idPaciente, agendamento.dataAgendamento, agendamento.horaAgendamento);

            DateTime horario = agendamento.dataAgendamento.ToDateTime(agendamento.horaAgendamento);

            var agendamentosExistentes = await _agendamentoRepository.ConsultarAgendamentosPorHorario(horario);

            if (agendamentosExistentes != null && agendamentosExistentes.Count > 0)
            {
                foreach (var agendamentoExistente in agendamentosExistentes)
                {
                    if (agendamentoExistente.idPaciente == agendamento.idPaciente)
                    {
                        _log.WarnFormat("O paciente {0} já está agendado para o horário selecionado", agendamento.idPaciente);
                        throw new BusinessException(string.Format(BusinessMessages.PacienteJaAgendado, agendamento.idPaciente));
                    }
                }

                await ValidarAgendamento(agendamento.dataAgendamento, agendamento.horaAgendamento, agendamento.idPaciente);
            }

            var novoAgendamento = CriarAgendamento(agendamento);

            var agendamentoInserido = await _agendamentoRepository.Inserir(novoAgendamento);
            var agendamentoInseridoDTO = new AgendamentoDTO
            {
                id = agendamentoInserido.Id,
                idPaciente = agendamentoInserido.idPaciente,
                dataAgendamento = agendamentoInserido.dataAgendamento,
                horaAgendamento = agendamentoInserido.horaAgendamento,
                status = agendamentoInserido.status,
                dataCriacao = agendamentoInserido.dataCriacao
            };

            return agendamentoInseridoDTO;
        }

        private static Agendamento CriarAgendamento(CadastroAgendamentoModel agendamentoModel)
        {
            return new Agendamento
            {
                idPaciente = agendamentoModel.idPaciente,
                dataAgendamento = agendamentoModel.dataAgendamento,
                horaAgendamento = agendamentoModel.horaAgendamento,
                status = StatusEnum.Pendente,
                dataCriacao = DateTime.Now
            };
        }


        public async Task<List<AgendamentoDTO>> AtualizarData(int id, DateTime novaData)
        {
            _log.InfoFormat("Iniciando atualização da data do agendamento ID: {0}", id);
            
            var agendamentoExistente = await _agendamentoRepository.ObterAgendamentoPorId(id);
            
            if (agendamentoExistente == null)
            {
                _log.WarnFormat("Tentativa de atualizar agendamento inexistente. ID: {0}", id);
                throw new BusinessException(string.Format(BusinessMessages.IdInvalido, id));
            }

            agendamentoExistente.dataAgendamento = DateOnly.FromDateTime(novaData);
            agendamentoExistente.horaAgendamento = TimeOnly.FromDateTime(novaData);

            var resultado = await _agendamentoRepository.Atualizar(agendamentoExistente);
            
            _log.InfoFormat("Agendamento ID: {0} atualizado com sucesso.", id);
            
            return await _agendamentoRepository.ListarAgendamentos();
        }

        public async Task<List<AgendamentoDTO>> AtualizarStatus(int id, StatusEnum novoStatus)
        {
            _log.InfoFormat("Iniciando atualização da data do agendamento ID: {0}", id);

            var agendamentoExistente = await _agendamentoRepository.ObterAgendamentoPorId(id);

            if (agendamentoExistente == null)
            {
                _log.WarnFormat("Tentativa de atualizar agendamento inexistente. ID: {0}", id);
                throw new BusinessException(string.Format(BusinessMessages.IdInvalido, id));
            }

            agendamentoExistente.status = novoStatus;

            var resultado = await _agendamentoRepository.Atualizar(agendamentoExistente);

            _log.InfoFormat("Agendamento ID: {0} atualizado com sucesso.", id);

            return await _agendamentoRepository.ListarAgendamentos();
        }

        public async Task Deletar(int agendamentoId)
        {
            _log.InfoFormat("Iniciando exclusão do agendamento ID: {0}", agendamentoId);

            var agendamento = await _agendamentoRepository.ObterAgendamentoPorId(agendamentoId);

            await _agendamentoRepository.Deletar(agendamento);
            
            _log.InfoFormat("Agendamento ID: {0} deletado com sucesso.", agendamento.Id);
        }

        public async Task Deletar(IEnumerable<int> agendamentosIds)
        {
            var agendamentosList = agendamentosIds.ToList();
            _log.InfoFormat("Iniciando exclusão de {0} agendamentos em lote.", agendamentosList.Count);
            var agendamentos = new List<Agendamento>();

            foreach (var id in agendamentosList)
            {
                var agendamento = await _agendamentoRepository.ObterAgendamentoPorId(id);
                if (agendamento == null)
                {
                    _log.WarnFormat("Agendamento com ID {0} não encontrado para exclusão em lote.", id);
                    throw new BusinessException(string.Format(BusinessMessages.IdInvalido, id));
                }
                agendamentos.Add(agendamento);
            }

            await _agendamentoRepository.Deletar(agendamentos);
            
            _log.InfoFormat("{0} agendamentos deletados com sucesso.", agendamentosList.Count);
        }


        public async Task ValidarAgendamento(DateOnly data, TimeOnly hora, int pacienteId)
        {
            var exceptions = new List<string>();

            if (data < DateOnly.FromDateTime(DateTime.Now))
            {
                exceptions.Add(string.Format(BusinessMessages.DataPassada));
            }

            if (hora.Minute != 0)
            {
                _log.WarnFormat("Tentativa de agendamento em horário inválido: {0}", hora);
                exceptions.Add(string.Format(BusinessMessages.HorarioInvalido, hora));
            }

            var agendamentosDoDia = await _agendamentoRepository.ConsultarAgendamentosPorDia(data);

            if (agendamentosDoDia.Count >= BusinessConstants.MAX_AGENDAMENTOS_POR_DIA)
            {
                _log.WarnFormat("Tentativa de agendamento excedendo limite diário. Data: {0}, Agendamentos: {1}", data, agendamentosDoDia.Count);
                exceptions.Add(string.Format(BusinessMessages.DiaEsgotado, BusinessConstants.MAX_AGENDAMENTOS_POR_DIA));
            }

            var agendamentosNoHorario = agendamentosDoDia
                .Where(a => a.horaAgendamento == hora)
                .ToList();

            if (agendamentosNoHorario.Count >= BusinessConstants.MAX_AGENDAMENTOS_POR_HORARIO)
            {
                _log.WarnFormat("Tentativa de agendamento excedendo limite horário. Data: {0}, Hora: {1}, Agendamentos: {2}", data, hora, agendamentosNoHorario.Count);
                exceptions.Add(string.Format(BusinessMessages.HorarioEsgotado, BusinessConstants.MAX_AGENDAMENTOS_POR_HORARIO));
            }

            if (exceptions.Any())
            {
                throw new BusinessListException(exceptions);
            }
        }
    }
}
