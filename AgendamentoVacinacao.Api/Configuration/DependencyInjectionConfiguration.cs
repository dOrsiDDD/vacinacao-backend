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
            throw new NotImplementedException();
        }

        private static void InjetarServico(IServiceCollection services)
        {
            throw new NotImplementedException();
        }

        private static void InjetarMiddleware(IServiceCollection services)
        {
            throw new NotImplementedException();
        }
    }
}
