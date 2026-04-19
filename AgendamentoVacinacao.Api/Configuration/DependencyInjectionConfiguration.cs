using AgendamentoVacinacao.Business.Business;
using AgendamentoVacinacao.Business.Interface.IBusiness;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Repository.Repositories;
using AgendamentoVacinacao.WebApi.Middleware;

namespace AgendamentoVacinacao.WebApi.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDependecyInjectionConfiguration(this IServiceCollection services, IConfiguration configuracao)
        {
            InjetarRepositorio(services);
            InjetarServico(services);
            InjetarMiddleware(services);
        }

        private static void InjetarRepositorio(IServiceCollection services)
        {
            services.AddScoped<IPacienteRepository, PacienteRepository>();
            services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();

        }

        private static void InjetarServico(IServiceCollection services)
        {
            services.AddScoped<IPacienteBusiness, PacienteBusiness>();
            services.AddScoped<IAgendamentoBusiness, AgendamentoBusiness>();
        }

        private static void InjetarMiddleware(IServiceCollection services)
        {
            services.AddTransient<ApiMiddleware>();
        }
    }
}
