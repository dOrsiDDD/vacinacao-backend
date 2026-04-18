using AgendamentoVacinacao.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgendamentoVacinacao.Repository
{
    public class Contexto : DbContext
    {
        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public Contexto(DbContextOptions<Contexto> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Contexto).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}

