using AgendamentoVacinacao.Business.Interface.IBusiness;
using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Entities.Model;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Repository.Repositories;
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

        public async Task<PacienteDTO> ObterPacientePorId(int id)
        {
            var paciente = await _pacienteRepository.ObterPacientePorId(id);

            if (paciente == null)
            {
                _log.InfoFormat("Paciente com ID {0} não encontrado.", id);
                throw new BusinessException(string.Format(BusinessMessages.IdInvalido, id));
            }

            _log.InfoFormat("Paciente com ID {0} obtido com sucesso. Nome: {1}", id, paciente.nome);

            return await _pacienteRepository.ConsultarPaciente(id);
        }

        public async Task<PacienteDTO> ObterPacientePorNome(string nome)
        {
            var paciente = await _pacienteRepository.ObterPacientePorNome(nome);

            if (paciente == null)
            {
                _log.InfoFormat("Paciente com nome '{0}' não encontrado.", nome);
                throw new BusinessException(string.Format(BusinessMessages.NomeInvalido, nome));
            }

            _log.InfoFormat("Paciente com nome '{0}' obtido com sucesso. ID: {1}", nome, paciente.Id);

            return await _pacienteRepository.ConsultarPaciente(paciente.Id);
        }

        public async Task<List<AgendamentoDTO>> ObterAgendamentosPorPaciente(int pacienteId)
        {
            var paciente = await _pacienteRepository.ObterPacientePorId(pacienteId);
            if (paciente == null)
            {
                _log.InfoFormat("Paciente com ID {0}.", pacienteId);
                throw new BusinessException(string.Format(BusinessMessages.IdInvalido, pacienteId));
            }

            var agendamentos = await _pacienteRepository.ObterAgendamentosPorPaciente(pacienteId);
            
            return agendamentos;
        }


        public async Task<List<PacienteDTO>> Inserir(CadastroPacienteModel novoPaciente)
        {
            _log.InfoFormat("Iniciando inserção de novo paciente. Nome: {0}", novoPaciente.nome);

            var paciente = await _pacienteRepository.ObterPacientePorNome(novoPaciente.nome);

            if (paciente != null)
            {
                _log.WarnFormat("Tentativa de inserir paciente com nome já existente: {0}", novoPaciente.nome);
                throw new BusinessException(string.Format(BusinessMessages.RegistroExistente, novoPaciente.nome));
            }

            paciente = CriarPaciente(novoPaciente);


            var resultado = await _pacienteRepository.Inserir(paciente);
            
            _log.InfoFormat("Paciente inserido com sucesso. ID: {0}, Nome: {1}", resultado.Id, resultado.nome);
            
            return await _pacienteRepository.ListarPacientes();
        }

        private static Paciente CriarPaciente(CadastroPacienteModel novoPaciente)
        {
            var paciente = new Paciente
            {
                nome = novoPaciente.nome,
                dataNascimento = novoPaciente.dataNascimento,
                dataCriacao = DateTime.Now
            };
            return paciente;
        }

        public async Task<PacienteDTO> AtualizarDataNascimento(string nome, DateOnly novaDataNascimento)
        {
            _log.InfoFormat("Iniciando atualização do paciente Nome: {0}", nome);
            
            var pacienteExistente = await _pacienteRepository.ObterPacientePorNome(nome);
            
            if (pacienteExistente == null)
            {
                _log.WarnFormat("Tentativa de atualizar paciente inexistente. Nome: {0}", nome);
                throw new BusinessException($"Paciente com Nome {nome} não encontrado.");
            }
            
            pacienteExistente.dataNascimento = novaDataNascimento;
            var resultado = await _pacienteRepository.Atualizar(pacienteExistente);
            
            _log.InfoFormat("Paciente de nome: {0} atualizado com sucesso.", pacienteExistente.nome);
            return await ObterPacientePorId(pacienteExistente.Id);
        }

        public async Task<PacienteDTO> AtualizarNome(string nomeAntigo, string nomeNovo)
        {
            _log.InfoFormat("Iniciando atualização do paciente Nome: {0}", nomeAntigo);
            
            var pacienteExistente = await _pacienteRepository.ObterPacientePorNome(nomeAntigo);
            
            if (pacienteExistente == null)
            {
                _log.WarnFormat("Tentativa de atualizar paciente inexistente. Nome: {0}", nomeAntigo);
                throw new BusinessException($"Paciente com Nome {nomeAntigo} não encontrado.");
            }
            
            pacienteExistente.nome = nomeNovo;
            var resultado = await _pacienteRepository.Atualizar(pacienteExistente);
            
            _log.InfoFormat("Paciente de nome: {0} atualizado com sucesso.", pacienteExistente.nome);
            return await ObterPacientePorId(pacienteExistente.Id);
        }

        public async Task<List<PacienteDTO>> Deletar(int pacienteId)
        {
            var pacienteExistente = await _pacienteRepository.ObterPacientePorId(pacienteId);
            
            if (pacienteExistente == null)
            {
                _log.WarnFormat("Tentativa de deletar paciente inexistente. ID: {0}", pacienteId);
                throw new BusinessException(string.Format(BusinessMessages.IdInvalido, pacienteId));
            }

            await _pacienteRepository.Deletar(pacienteExistente);

            _log.InfoFormat("Paciente ID: {0} deletado com sucesso.", pacienteId);

            return await _pacienteRepository.ListarPacientes();
        }
    }
}
