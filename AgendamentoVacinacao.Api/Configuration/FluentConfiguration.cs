using AgendamentoVacinacao.Validator.FluentValidator;
using FluentValidation.AspNetCore;


namespace AgendamentoVacinacao.WebApi.Configuration
{
    public static class FluentConfiguration
    {
        public static void AddFluentConfiguration(this IServiceCollection services)
        {
            services.AddFluentValidation(c => c.RegisterValidatorsFromAssemblyContaining<CadastroPacienteValidator>());
            services.AddFluentValidation(c => c.RegisterValidatorsFromAssemblyContaining<CadastroAgendamentoValidator>());
        }
    }
}
