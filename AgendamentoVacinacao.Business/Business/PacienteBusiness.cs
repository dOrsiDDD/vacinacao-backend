using AgendamentoVacinacao.Business.Interface.IBusiness;
using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Utilities.Exceptions;
using AgendamentoVacinacao.Utilities.Messages;
using log4net;

namespace AgendamentoVacinacao.Business.Business
{
    public class PacienteBusiness : IPacienteBusiness
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(PacienteBusiness));
        private readonly IPacienteRepository _pacienteRepository;

        public PacienteBusiness(IPacienteRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository;
        }

        public async Task<List<PacienteDTO>> ListarPacientes()
        {
            var pacientes = await _pacienteRepository.ListarPacientes();

            if (pacientes == null || pacientes.Count == 0)
            {
                _log.InfoFormat("Nenhum paciente encontrado na base de dados.");
            }
            else
            {
                _log.InfoFormat("Total de {0} pacientes listados.", pacientes.Count);
            }

            return pacientes;
        }

        public async Task<Paciente> ObterPacientePorId(int id)
        {
            ValidarId(id);

            var paciente = await _pacienteRepository.ObterPacientePorId(id);

            if (paciente == null)
            {
                _log.InfoFormat("Paciente com ID {0} não encontrado.", id);
                throw new BusinessException(string.Format(BusinessMessages.IdInvalido, id));
            }

            _log.InfoFormat("Paciente com ID {0} obtido com sucesso. Nome: {1}", id, paciente.nome);
            return paciente;
        }

        public async Task<Paciente> ObterPacientePorNome(string nome)
        {
            ValidarNome(nome);

            var paciente = await _pacienteRepository.ObterPacientePorNome(nome);

            if (paciente == null)
            {
                _log.InfoFormat("Paciente com nome '{0}' não encontrado.", nome);
                throw new BusinessException(string.Format(BusinessMessages.NomeInvalido, nome));
            }

            _log.InfoFormat("Paciente com nome '{0}' obtido com sucesso. ID: {1}", nome, paciente.Id);
            return paciente;
        }

        private static void ValidarId(int id)
        {
            if (id <= 0)
            {
                throw new BusinessException(BusinessMessages.IdNegativo);
            }
        }

        private static void ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new BusinessException("Nome do paciente é obrigatório.");
            }
        }
    }
}
