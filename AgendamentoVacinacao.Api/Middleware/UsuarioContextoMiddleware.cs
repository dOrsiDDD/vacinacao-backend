using AgendamentoVacinacao.Utilities.Extensions;
using AgendamentoVacinacao.Utilities.UsuarioContexto;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AgendamentoVacinacao.WebApi.Middleware
{
    public class UsuarioContextoMiddleware : IMiddleware
    {
        private readonly IUsuarioContexto _usuarioContexto;

        public UsuarioContextoMiddleware(IUsuarioContexto usuarioContexto)
        {
            _usuarioContexto = usuarioContexto;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (SetUser(context))
                await next.Invoke(context);
        }

        private bool SetUser(HttpContext context)
        {
            if (IsAuthenticated(context))
            {
                var securityToken = GetSecurityToken(context);

                SetUserContext(context, securityToken);

                return true;
            }
            else
            {
                throw new UnauthorizedAccessException("Usuário não autorizado");
            }
        }

        private static JwtSecurityToken GetSecurityToken(HttpContext context)
        {
            var authToken = context.Request.Headers["Authorization"].String();

            if (authToken != null && authToken.Trim().Length > 0)
            {
                var token = authToken.Replace("Bearer", string.Empty).Trim();
                return new JwtSecurityTokenHandler().ReadJwtToken(token);
            }

            return null;
        }

        private static bool IsAuthenticated(HttpContext context)
        {
            var authToken = context.Request.Headers["Authorization"].String();

            if (!string.IsNullOrEmpty(authToken))
                return (context.User?.Identity?.IsAuthenticated ?? false) || !string.IsNullOrEmpty(authToken);
            else
                return true;
        }

        private void SetUserContext(HttpContext context, JwtSecurityToken securityToken)
        {
            _usuarioContexto.RequestId = Guid.NewGuid();
            _usuarioContexto.StartDateTime = DateTime.UtcNow;
            _usuarioContexto.SourceInfo = new SourceInfo
            {
                IP = context?.Connection?.RemoteIpAddress,
                Data = GetAllHeaders(context)
            };

            if (securityToken != null && securityToken.Claims.Any())
            {
                var userName = securityToken.Claims.GetClaimValue(ClaimTypes.Name);
                var roles = securityToken.Claims.GetValuesOfType(ClaimTypes.Role);
                var login = securityToken.Claims.GetClaimValue("login");

                _usuarioContexto.AddData("userName", userName);
                _usuarioContexto.AddData("roles", roles);
                _usuarioContexto.AddData("login", login);
            }
        }

        private static Hashtable GetAllHeaders(HttpContext context)
        {
            var hashtable = new Hashtable();
            var requestHeaders = context?.Request?.Headers;

            if (requestHeaders == null)
                return hashtable;

            foreach (var header in requestHeaders)
                hashtable.Add(header.Key, header.Value);

            return hashtable;
        }
    }
}