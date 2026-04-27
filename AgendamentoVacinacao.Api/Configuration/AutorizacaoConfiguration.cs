using AgendamentoVacinacao.Utilities.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using tusdotnet.Helpers;

namespace AgendamentoVacinacao.WebApi.Configuration
{
    public static class AutorizacaoConfiguration
    {
        public static void AddAutorizacaoConfiguration(this IServiceCollection services, IConfiguration configuracao)
        {
            var autenticacaoConfig = new AutenticacaoConfig
            {
                Issuer = configuracao["Authorization:Issuer"],
                Audience = configuracao["Authorization:Audience"],
                SecretKey = configuracao["Authorization:SecretKey"],
                AccessTokenExpiration = int.Parse(configuracao["Authorization:AccessTokenExpiration"]),
                RefreshTokenExpiration = int.Parse(configuracao["Authorization:RefreshTokenExpiration"]),
            };

            services.AddCors(o => o.AddPolicy("CORS_POLICY", builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowAnyOrigin();
            }));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = autenticacaoConfig.Issuer,

                            ValidateAudience = true,
                            ValidAudience = autenticacaoConfig.Audience,

                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(autenticacaoConfig.SecretKey)),

                            RequireExpirationTime = true,
                            ValidateLifetime = true,

                            ClockSkew = TimeSpan.Zero
                        };
                    });
        }

    }
}
