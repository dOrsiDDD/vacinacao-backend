using AgendamentoVacinacao.Business.Interface.IBusiness;
using AgendamentoVacinacao.Entities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendamentoVacinacao.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly IAutenticacaoNegocio _autenticacaoNegocio;

        public AutenticacaoController(IAutenticacaoNegocio autenticacaoNegocio)
        {
            _autenticacaoNegocio = autenticacaoNegocio;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<TokenUsuarioDTO> Login([FromBody] LoginRequest request)
        {
            return await _autenticacaoNegocio.Login(request.login, request.senha);
        }

        [HttpGet("refreshToken")]
        [ProducesResponseType(typeof(TokenUsuarioDTO), StatusCodes.Status200OK)]
        [Authorize]
        public async Task<TokenUsuarioDTO> RefreshToken()
        {
            return await _autenticacaoNegocio.RefreshToken();

        }
    }
}