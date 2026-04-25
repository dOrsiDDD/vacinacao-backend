using AgendamentoVacinacao.Business.Interface.IBusiness;
using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Entities.Model;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Repository.Repositories;
using AgendamentoVacinacao.Utilities.Exceptions;
using AgendamentoVacinacao.Utilities.Messages;
using log4net;
using System.Drawing;

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

        public async Task<PacienteDTO> ObterPacientePorCPF(string cpf)
        {
            var paciente = await _pacienteRepository.ObterPacientePorCPF(cpf);

            if (paciente == null)
            {
                _log.InfoFormat("Paciente com CPF {0} não encontrado.", cpf);
                throw new BusinessException(string.Format(BusinessMessages.CPFInvalido, cpf));
            }

            _log.InfoFormat("Paciente com CPF {0} obtido com sucesso. Nome: {1}", cpf, paciente.nome);

            return await _pacienteRepository.ConsultarPaciente(cpf);
        }

        public async Task<List<PacienteDTO>> ObterPacientesPorNome(string nome)
        {
            var pacientes = await _pacienteRepository.ObterPacientesPorNome(nome);

            if (pacientes == null || pacientes.Count == 0)
            {
                _log.InfoFormat("Paciente com nome '{0}' não encontrado.", nome);
                throw new BusinessException(string.Format(BusinessMessages.NomeInvalido, nome));
            }

            _log.InfoFormat("Pacientes com nome '{0}' obtidos com sucesso. Total: {1}", nome, pacientes.Count);

            var pacientesDTO = new List<PacienteDTO>();

            foreach (var paciente in pacientes)
            {
                var pacienteDTO = await _pacienteRepository.ConsultarPaciente(paciente.cpf);
                if (pacienteDTO != null)
                {
                    pacientesDTO.Add(pacienteDTO);
                }
            }

            return pacientesDTO;
        }

        public async Task<List<AgendamentoDTO>> ObterAgendamentosPorPaciente(string cpf)
        {
            var paciente = await _pacienteRepository.ObterPacientePorCPF(cpf);
            if (paciente == null)
            {
                _log.InfoFormat("Paciente com CPF {0} não encontrado.", cpf);
                throw new BusinessException(string.Format(BusinessMessages.CPFInvalido, cpf));
            }

            var agendamentos = await _pacienteRepository.ObterAgendamentosPorPaciente(paciente.cpf);

            return agendamentos;
        }


        public async Task<List<PacienteDTO>> Inserir(CadastroPacienteModel novoPaciente)
        {
            _log.InfoFormat("Iniciando inserção de novo paciente. Nome: {0}", novoPaciente.nome);

            ValidarDataNascimento(novoPaciente.dataNascimento);

            var paciente = CriarPaciente(novoPaciente);


            var resultado = await _pacienteRepository.Inserir(paciente);

            _log.InfoFormat("Paciente inserido com sucesso. ID: {0}, Nome: {1}", resultado.Id, resultado.nome);

            return await _pacienteRepository.ListarPacientes();
        }

        private static Paciente CriarPaciente(CadastroPacienteModel novoPaciente)
        {
            var paciente = new Paciente
            {
                nome = novoPaciente.nome,
                cpf = novoPaciente.cpf,
                dataNascimento = novoPaciente.dataNascimento,
                dataCriacao = DateTime.Now
            };
            return paciente;
        }

        public async Task<PacienteDTO> AtualizarDataNascimento(string cpf, DateOnly novaDataNascimento)
        {
            _log.InfoFormat("Iniciando atualização do paciente ID: {0}", cpf);

            ValidarDataNascimento(novaDataNascimento);

            var pacienteExistente = await _pacienteRepository.ObterPacientePorCPF(cpf);

            if (pacienteExistente == null)
            {
                _log.WarnFormat("Tentativa de atualizar paciente inexistente. CPF: {0}", cpf);
                throw new BusinessException(string.Format(BusinessMessages.CPFInvalido, cpf));
            }

            pacienteExistente.dataNascimento = novaDataNascimento;
            var resultado = await _pacienteRepository.Atualizar(pacienteExistente);

            _log.InfoFormat("Paciente de nome: {0} atualizado com sucesso.", pacienteExistente.nome);
            return await ObterPacientePorCPF(pacienteExistente.cpf);
        }

        public async Task<PacienteDTO> AtualizarNome(string cpf, string nomeNovo)
        {
            _log.InfoFormat("Iniciando atualização do paciente ID: {0}", cpf);

            var pacienteExistente = await _pacienteRepository.ObterPacientePorCPF(cpf);

            if (pacienteExistente == null)
            {
                _log.WarnFormat("Tentativa de atualizar paciente inexistente. CPF: {0}", cpf);
                throw new BusinessException(string.Format(BusinessMessages.CPFInvalido, cpf));
            }

            pacienteExistente.nome = nomeNovo;
            var resultado = await _pacienteRepository.Atualizar(pacienteExistente);

            _log.InfoFormat("Paciente de nome: {0} atualizado com sucesso.", pacienteExistente.nome);
            return await ObterPacientePorCPF(pacienteExistente.cpf);
        }

        public async Task<List<PacienteDTO>> Deletar(string cpf)
        {
            var pacienteExistente = await _pacienteRepository.ObterPacientePorCPF(cpf);

            if (pacienteExistente == null)
            {
                _log.WarnFormat("Tentativa de deletar paciente inexistente. CPF: {0}", cpf);
                throw new BusinessException(string.Format(BusinessMessages.CPFInvalido, cpf));
            }

            await _pacienteRepository.Deletar(pacienteExistente);

            _log.InfoFormat("Paciente CPF: {0} deletado com sucesso.", cpf);

            return await _pacienteRepository.ListarPacientes();
        }

        public void ValidarDataNascimento(DateOnly dataNascimento)
        {
            if (dataNascimento > DateOnly.FromDateTime(DateTime.Now))
            {
                _log.WarnFormat("Data de nascimento inválida: {0}", dataNascimento);
                throw new BusinessException(BusinessMessages.DataNascimentoFuturo);
            }

            if (dataNascimento < new DateOnly(1900, 1, 1))
            {
                _log.WarnFormat("Data de nascimento muito antiga: {0}", dataNascimento);
                throw new BusinessException(BusinessMessages.DataNascimentoInvalida);
            }
        }
    }
}
