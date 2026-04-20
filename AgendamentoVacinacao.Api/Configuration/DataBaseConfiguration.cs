using AgendamentoVacinacao.Repository;
using Microsoft.EntityFrameworkCore;

namespace AgendamentoVacinacao.WebApi.Configuration
{
    public static class DataBaseConfiguration
    {
        public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Contexto>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), 
                                           x => x.UseDateOnlyTimeOnly()));
        }
    }
}
