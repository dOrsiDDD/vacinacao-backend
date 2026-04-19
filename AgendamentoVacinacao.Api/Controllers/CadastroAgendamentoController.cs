using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Model;
using AgendamentoVacinacao.Business.Interface.IBusiness;
using Microsoft.AspNetCore.Mvc;
using AgendamentoVacinacao.Entities.Enum;

namespace AgendamentoVacinacao.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CadastroAgendamentoController : ControllerBase
    {
        private readonly IAgendamentoBusiness _agendamentoBusiness;

        public CadastroAgendamentoController(IAgendamentoBusiness agendamentoBusiness)
        {
            _agendamentoBusiness = agendamentoBusiness;
        }

        [HttpPost("CadastrarAgendamento")]
        public async Task<List<AgendamentoDTO>> CadastrarAgendamento(CadastroAgendamentoModel novoAgendamento)
        {
            return await _agendamentoBusiness.Inserir(novoAgendamento);
        }

        [HttpGet("ListarAgendamentos")]
        public async Task<List<AgendamentoDTO>> ListarAgendamentos()
        {
            return await _agendamentoBusiness.ListarAgendamentos();
        }

        [HttpGet("ConsultarAgendamentosPorDia")]
        public async Task<List<AgendamentoDTO>> ConsultarAgendamentosPorDia(DateOnly dia)
        {
            return await _agendamentoBusiness.ConsultarAgendamentosPorDia(dia);
        }

        [HttpGet("ConsultarAgendamentosPorHorario")]
        public async Task<List<AgendamentoDTO>> ConsultarAgendamentosPorHorario(DateTime horario)
        {
            return await _agendamentoBusiness.ConsultarAgendamentosPorHorario(horario);
        }

        [HttpGet("FiltrarAgendamentos")]
        public async Task<List<AgendamentoDTO>> FiltrarAgendamentos(StatusEnum status)
        {
            return await _agendamentoBusiness.FiltrarAgendamentos(status);
        }

        [HttpPut("AtualizarData")]
        public async Task<List<AgendamentoDTO>> AtualizarData(int id, DateTime novaData)
        {
            return await _agendamentoBusiness.AtualizarData(id, novaData);
        }

        [HttpPut("AtualizarStatus")]
        public async Task<List<AgendamentoDTO>> AtualizarStatus(int id, StatusEnum novoStatus)
        {
            return await _agendamentoBusiness.AtualizarStatus(id, novoStatus);
        }

        [HttpDelete("DeletarAgendamento")]
        public async Task DeletarAgendamento(int id)
        {
            await _agendamentoBusiness.Deletar(id);
        }

        [HttpDelete("DeletarAgendamentos")]
        public async Task DeletarAgendamentos(IEnumerable<int> agendamentosIds)
        {
            await _agendamentoBusiness.Deletar(agendamentosIds);
        }
    }
}
