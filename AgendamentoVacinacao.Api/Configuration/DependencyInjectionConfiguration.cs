using AgendamentoVacinacao.Business.Business;
using AgendamentoVacinacao.Business.Interface.IBusiness;
using AgendamentoVacinacao.Repositorio;
using AgendamentoVacinacao.Repositorio.Interface;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Repository.Repositories;
using AgendamentoVacinacao.Utilities.Configuration;
using AgendamentoVacinacao.Utilities.UsuarioContexto;
using AgendamentoVacinacao.WebApi.Middleware;

namespace AgendamentoVacinacao.WebApi.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDependecyInjectionConfiguration(this IServiceCollection services, IConfiguration configuracao)
        {
            InjetarRepositorio(services);
            InjetarNegocio(services);
            InjetarMiddleware(services);

            services.AddScoped<IGerenciadorTransacao, GerenciadorTransacao>();
            services.AddScoped<IUsuarioContexto, UsuarioContexto>();
            services.AddOptions<AutenticacaoConfig>().Bind(configuracao.GetSection("Authorization"));
        }

        private static void InjetarRepositorio(IServiceCollection services)
        {
            services.AddScoped<IPacienteRepository, PacienteRepository>();
            services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        }

        private static void InjetarNegocio(IServiceCollection services)
        {
            services.AddScoped<IPacienteBusiness, PacienteBusiness>();
            services.AddScoped<IAgendamentoBusiness, AgendamentoBusiness>();
            services.AddScoped<IAutenticacaoNegocio, AutenticacaoNegocio>();
            services.AddScoped<IUsuarioBusiness, UsuarioBusiness>();
        }

        private static void InjetarMiddleware(IServiceCollection services)
        {
            services.AddTransient<ApiMiddleware>();
            services.AddTransient<UsuarioContextoMiddleware>();
        }
    }
}
