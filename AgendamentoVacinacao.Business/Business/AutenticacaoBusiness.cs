using AgendamentoVacinacao.Business.Interface.IBusiness;
using AgendamentoVacinacao.Utilities.Extensions;
using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Entities.Filter;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Utilities.Configuration;
using AgendamentoVacinacao.Utilities.Messages;
using AgendamentoVacinacao.Utilities.UsuarioContexto;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoVacinacao.Business.Business
{
    public class AutenticacaoNegocio : IAutenticacaoNegocio
    {
        private readonly IUsuarioRepository _usuarioRepositorio;
        private readonly AutenticacaoConfig _autenticacaoConfig;
        private readonly IUsuarioContexto _usuarioContexto;
        public AutenticacaoNegocio(IUsuarioRepository usuarioRepositorio,
                                   IOptionsMonitor<AutenticacaoConfig> autenticacaoConfig,
                                   IUsuarioContexto usuarioContexto)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _autenticacaoConfig = autenticacaoConfig.CurrentValue;
            _usuarioContexto = usuarioContexto;
        }

        public async Task<TokenUsuarioDTO> Login(string login, string senha)
        {
            var usuarioValido = await Autenticar(login, senha);
            var usuario = await _usuarioRepositorio.ObterUsuario(new UsuarioFilter { login = login });
            string token;
            string refreshToken;

            if (usuarioValido && usuario != null)
            {
                token = GerarToken(usuario);
                refreshToken = GerarRefreshToken(usuario);
            }
            else
                throw new UnauthorizedAccessException(BusinessMessages.UsuarioSenhaInvalida);

            return new TokenUsuarioDTO(token, refreshToken);
        }

        public async Task<TokenUsuarioDTO> RefreshToken()
        {
            var login = _usuarioContexto.Login();
            var usuario = await _usuarioRepositorio.ObterUsuario(new UsuarioFilter { login = login });
            string token;
            string refreshToken;

            if (usuario != null)
            {
                token = GerarToken(usuario);
                refreshToken = GerarRefreshToken(usuario);
            }
            else
                throw new UnauthorizedAccessException();

            return new TokenUsuarioDTO(token, refreshToken);
        }

        public async Task<bool> Autenticar(string login, string senha)
        {
            var user = await _usuarioRepositorio.ObterUsuario(new UsuarioFilter { login = login });

            if (user == null)
                return false;

            using var hmac = new HMACSHA512(user.passwordSalt);

            return hmac.ComputeHash(Encoding.UTF8.GetBytes(senha))
                       .SequenceEqual(user.passwordHash);
        }

        public string GerarToken(Usuario usuario)
        {
            var expiration = DateTime.Now.AddMinutes(_autenticacaoConfig.AccessTokenExpiration);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Sid, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.nome),
                new(ClaimTypes.Role, usuario.perfil.ToString()),
                new("login", usuario.login),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var privateKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_autenticacaoConfig.SecretKey));
            var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _autenticacaoConfig.Issuer,
                audience: _autenticacaoConfig.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GerarRefreshToken(Usuario usuario)
        {
            var expiration = DateTime.Now.AddMinutes(_autenticacaoConfig.RefreshTokenExpiration);

            var claims = new List<Claim>
            {
                new("login", usuario.login)
            };

            var privateKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_autenticacaoConfig.SecretKey));
            var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _autenticacaoConfig.Issuer,
                audience: _autenticacaoConfig.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
