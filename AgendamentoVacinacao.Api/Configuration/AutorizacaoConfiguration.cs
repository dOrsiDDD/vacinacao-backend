namespace AgendamentoVacinacao.WebApi.Configuration
{
    public static class AutorizacaoConfiguration
    {
        public static void AddAutorizacaoConfiguration(this IServiceCollection services, IConfiguration configuracao)
        {
            services.AddCors(o => o.AddPolicy("CORS_POLICY", builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowAnyOrigin();
            }));
        }

    }
}
