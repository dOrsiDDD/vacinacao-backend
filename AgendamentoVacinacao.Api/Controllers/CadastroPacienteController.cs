using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Model;
using AgendamentoVacinacao.Business.Interface.IBusiness;

using Microsoft.AspNetCore.Mvc;

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
        public async Task<List<PacienteDTO>> CadastrarPaciente(CadastroPacienteModel novoPaciente)
        {
            return await _pacienteBusiness.Inserir(novoPaciente);
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
        public async Task<List<PacienteDTO>> DeletarPaciente(string cpf)
        {
            return await _pacienteBusiness.Deletar(cpf);
        }

        [HttpPut("AtualizarDataNascimento")]
        public async Task<PacienteDTO> AtualizarDataNascimento(string cpf, DateOnly novaDataNascimento)
        {
            return await _pacienteBusiness.AtualizarDataNascimento(cpf, novaDataNascimento);
        }

        [HttpPut("AtualizarNome")]
        public async Task<PacienteDTO> AtualizarNome(string cpf, string nomeNovo)
        {
            return await _pacienteBusiness.AtualizarNome(cpf, nomeNovo);
        }
    }
}
