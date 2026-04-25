using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Model;
using AgendamentoVacinacao.Business.Interface.IBusiness;

using Microsoft.AspNetCore.Mvc;
using AgendamentoVacinacao.Utilitarios.Attributes;

namespace AgendamentoVacinacao.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CadastroPacienteController : ControllerBase
    {
        private readonly IPacienteBusiness _pacienteBusiness;

        public CadastroPacienteController(IPacienteBusiness pacienteBusiness)
        {
            _pacienteBusiness = pacienteBusiness;
        }

        [HttpPost("CadastrarPaciente")]
        [TransacaoObrigatoria]
        public async Task<PacienteDTO> CadastrarPaciente(CadastroPacienteModel novoPaciente)
        {
            return await _pacienteBusiness.Inserir(novoPaciente);
        }

        [HttpGet("ListarPacientes")]
        public async Task<List<PacienteDTO>> ListarPacientes()
        {
            return await _pacienteBusiness.ListarPacientes();
        }

        [HttpGet("ConsultarAgendamentoPorPaciente")]
        public async Task<List<AgendamentoDTO>> ConsultarAgendamentosPorPaciente(string cpf)
        {
            return await _pacienteBusiness.ObterAgendamentosPorPaciente(cpf);
        }

        [HttpGet("ConsultarPacientePorCPF")]
        public async Task<PacienteDTO> ConsultarPacientePorCPF(string cpf)
        {
            return await _pacienteBusiness.ObterPacientePorCPF(cpf);
        }

        [HttpGet("ConsultarPacientePorNome")]
        public async Task<List<PacienteDTO>> ConsultarPacientesPorNome(string nome)
        {
            return await _pacienteBusiness.ObterPacientesPorNome(nome);
        }

        [HttpDelete("DeletarPaciente")]
        [TransacaoObrigatoria]
        public async Task<List<PacienteDTO>> DeletarPaciente(string cpf)
        {
            return await _pacienteBusiness.Deletar(cpf);
        }

        [HttpPut("AtualizarDataNascimento")]
        [TransacaoObrigatoria]
        public async Task<PacienteDTO> AtualizarDataNascimento(string cpf, DateOnly novaDataNascimento)
        {
            return await _pacienteBusiness.AtualizarDataNascimento(cpf, novaDataNascimento);
        }

        [HttpPut("AtualizarNome")]
        [TransacaoObrigatoria]
        public async Task<PacienteDTO> AtualizarNome(string cpf, string nomeNovo)
        {
            return await _pacienteBusiness.AtualizarNome(cpf, nomeNovo);
        }
    }
}
