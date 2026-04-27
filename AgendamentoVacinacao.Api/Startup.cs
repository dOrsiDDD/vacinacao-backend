using AgendamentoVacinacao.WebApi.Configuration;
using AgendamentoVacinacao.WebApi.Middleware;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace AgendamentoVacinacao.WebApi
{
    public class Startup
    {
        public IConfiguration Configuracao { get; }
        public Startup(IConfiguration configuracao)
        {
            Configuracao = configuracao;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
                });

            services.AddDependecyInjectionConfiguration(Configuracao);

            services.AddDatabaseConfiguration(Configuracao);

            services.AddFluentConfiguration();
            services.AddAutorizacaoConfiguration(Configuracao);

            services.AddSwaggerGen(c =>
            {
                c.MapType(typeof(TimeSpan), () => new() { Type = "string", Example = new OpenApiString("00:00:00") });
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Agendamento de Vacinação",
                    Version = "v1",
                    Description = "APIs para gerenciar o agendamento de vacinações de covid-19.",
                    Contact = new() { Name = "Diego Duarte", Url = new Uri("http://google.com.br") },
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Insira o token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new() { { new() { Reference = new() { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() } });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
                if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Agendamento de Vacinação v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseCors("CORS_POLICY");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<ApiMiddleware>();
            app.UseMiddleware<UsuarioContextoMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

