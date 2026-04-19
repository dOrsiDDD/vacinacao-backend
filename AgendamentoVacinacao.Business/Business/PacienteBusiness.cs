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
            var paciente = await _pacienteRepository.ObterPacientePorNome(nome);

            if (paciente == null)
            {
                _log.InfoFormat("Paciente com nome '{0}' não encontrado.", nome);
                throw new BusinessException(string.Format(BusinessMessages.NomeInvalido, nome));
            }

            _log.InfoFormat("Paciente com nome '{0}' obtido com sucesso. ID: {1}", nome, paciente.Id);
            return paciente;
        }


        public async Task<Paciente> Inserir(Paciente paciente)
        {
            _log.InfoFormat("Iniciando inserção de novo paciente. Nome: {0}", paciente.nome);
            
            paciente.dataCriacao = DateTime.Now;
            
            var resultado = await _pacienteRepository.Inserir(paciente);
            
            _log.InfoFormat("Paciente inserido com sucesso. ID: {0}, Nome: {1}", resultado.Id, resultado.nome);
            
            return resultado;
        }

        public async Task Inserir(IEnumerable<Paciente> pacientes)
        {
            var pacientesList = pacientes.ToList();
            
            _log.InfoFormat("Iniciando inserção de {0} pacientes em lote.", pacientesList.Count);
            
            foreach (var paciente in pacientesList)
            {
                paciente.dataCriacao = DateTime.Now;
            }
            
            await _pacienteRepository.Inserir(pacientesList);
            
            _log.InfoFormat("{0} pacientes inseridos com sucesso.", pacientesList.Count);
        }

        public async Task<Paciente> Atualizar(Paciente paciente)
        {
            _log.InfoFormat("Iniciando atualização do paciente ID: {0}", paciente.Id);
            
            var pacienteExistente = await _pacienteRepository.ObterPacientePorId(paciente.Id);
            
            if (pacienteExistente == null)
            {
                _log.WarnFormat("Tentativa de atualizar paciente inexistente. ID: {0}", paciente.Id);
                throw new BusinessException($"Paciente com ID {paciente.Id} não encontrado.");
            }
            
            var resultado = await _pacienteRepository.Atualizar(paciente);
            
            _log.InfoFormat("Paciente ID: {0} atualizado com sucesso.", paciente.Id);
            
            return resultado;
        }

        public async Task Deletar(Paciente paciente)
        {
            _log.InfoFormat("Iniciando exclusão do paciente ID: {0}", paciente.Id);
            
            await _pacienteRepository.Deletar(paciente);
            
            _log.InfoFormat("Paciente ID: {0} deletado com sucesso.", paciente.Id);
        }

        public async Task Deletar(IEnumerable<Paciente> pacientes)
        {
            var pacientesList = pacientes.ToList();
            
            _log.InfoFormat("Iniciando exclusão de {0} pacientes em lote.", pacientesList.Count);
            
            await _pacienteRepository.Deletar(pacientesList);
            
            _log.InfoFormat("{0} pacientes deletados com sucesso.", pacientesList.Count);
        }

        public async Task DeletarPorId(int id)
        {
            if (id <= 0)
            {
                _log.WarnFormat("Tentativa de deletar paciente com ID inválido: {0}", id);
                throw new BusinessException(string.Format(BusinessMessages.IdInvalido, id));
            }
            
            var paciente = await _pacienteRepository.ObterPacientePorId(id);
            
            if (paciente == null)
            {
                _log.WarnFormat("Tentativa de deletar paciente inexistente. ID: {0}", id);
                throw new BusinessException(string.Format(BusinessMessages.IdInvalido, id));
            }
            
            _log.InfoFormat("Iniciando exclusão do paciente ID: {0}", id);
            
            await _pacienteRepository.DeletarPorId(id);
            
            _log.InfoFormat("Paciente ID: {0} deletado com sucesso.", id);
        }

        public async Task<List<Paciente>> Todos()
        {
            _log.InfoFormat("Obtendo lista completa de pacientes.");
            
            var pacientes = await _pacienteRepository.Todos();
            
            _log.InfoFormat("Total de {0} pacientes recuperados.", pacientes.Count);
            
            return pacientes;
        }
    }
}
