using AgendamentoVacinacao.Business.Interface.IBusiness;
using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Enum;
using AgendamentoVacinacao.Entities.Filter;
using AgendamentoVacinacao.Entities.Model;
using AgendamentoVacinacao.Utilitarios.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleTarefas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class CadastroUsuarioController : ControllerBase
    {
        private readonly IUsuarioBusiness _usuarioBusiness;
        public CadastroUsuarioController(IUsuarioBusiness usuarioBusiness)
        {
            _usuarioBusiness = usuarioBusiness;
        }

        [HttpPost("InserirUsuario")]
        [TransacaoObrigatoria]
        public async Task<UsuarioDTO> InserirUsuario(CadastroUsuarioModel novoUsuario)
        {
            return await _usuarioBusiness.InserirUsuario(novoUsuario);
        }

        [HttpGet("ConsultarUsuario")]
        public async Task<UsuarioDTO> ConsultarUsuario(UsuarioFilter filtro)
        {
            return await _usuarioBusiness.ConsultarUsuario(filtro);
        }

        [HttpGet("ListarUsuarios")]
        [AllowAnonymous]
        public async Task<List<UsuarioDTO>> ListarUsuarios()
        {
            return await _usuarioBusiness.ListarTodos();
        }
    }
}
