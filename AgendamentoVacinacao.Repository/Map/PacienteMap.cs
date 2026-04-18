using AgendamentoVacinacao.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendamentoVacinacao.Repository.Map
{
    public class PacienteMap : IEntityTypeConfiguration<Paciente>
    {
        public void Configure(EntityTypeBuilder<Paciente> builder)
        {
            builder.ToTable("tb_paciente");

            builder.HasKey(e => e.Id);

            builder.Property(e=>e.Id)
                   .HasColumnName("id_paciente")
                   .IsRequired();

            builder.Property(e => e.nome)
                   .HasColumnName("dsc_nome")
                   .IsRequired();

            builder.Property(e => e.dataNascimento)
                   .HasColumnName("dat_nascimento")
                   .IsRequired();

            builder.Property(e => e.dataCriacao)
                   .HasColumnName("dat_criacao")
                   .IsRequired();

            builder.HasMany(p => p.agendamentos)
                   .WithOne(a => a.paciente)
                   .HasForeignKey(a => a.idPaciente)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

