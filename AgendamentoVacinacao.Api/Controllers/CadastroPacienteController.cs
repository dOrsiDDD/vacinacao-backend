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
        public async Task<List<AgendamentoDTO>> ConsultarAgendamentosPorPaciente(int id)
        {
            return await _pacienteBusiness.ObterAgendamentosPorPaciente(id);
        }

        [HttpGet("ConsultarPacientePorId")]
        public async Task<PacienteDTO> ConsultarPacientePorId(int id)
        {
            return await _pacienteBusiness.ObterPacientePorId(id);
        }

        [HttpGet("ConsultarPacientePorNome")]
        public async Task<List<PacienteDTO>> ConsultarPacientesPorNome(string nome)
        {
            return await _pacienteBusiness.ObterPacientesPorNome(nome);
        }

        [HttpDelete("DeletarPaciente")]
        public async Task<List<PacienteDTO>> DeletarPaciente(int id)
        {
            return await _pacienteBusiness.Deletar(id);
        }

        [HttpPut("AtualizarDataNascimento")]
        public async Task<PacienteDTO> AtualizarDataNascimento(int id, DateOnly novaDataNascimento)
        {
            return await _pacienteBusiness.AtualizarDataNascimento(id, novaDataNascimento);
        }

        [HttpPut("AtualizarNome")]
        public async Task<PacienteDTO> AtualizarNome(int id, string nomeNovo)
        {
            return await _pacienteBusiness.AtualizarNome(id, nomeNovo);
        }
    }
}
