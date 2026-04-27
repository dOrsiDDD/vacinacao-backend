using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendamentoVacinacao.Entities.DTO
{
    public class TokenUsuarioDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public TokenUsuarioDTO(string token, string refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
        }
    }
}
