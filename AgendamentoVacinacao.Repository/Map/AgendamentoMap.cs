using AgendamentoVacinacao.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendamentoVacinacao.Repository.Map
{
    public class AgendamentoMap : IEntityTypeConfiguration<Agendamento>
    {
        public void Configure(EntityTypeBuilder<Agendamento> builder) 
        {
            builder.ToTable("tb_agendamento");

            builder.HasKey(e => e.Id);

            builder.Property(e=>e.Id)
                   .HasColumnName("id_agendamento")
                   .IsRequired();

            builder.Property(e => e.idPaciente)
                   .HasColumnName("id_paciente")
                   .IsRequired();

            builder.Property(e => e.dataAgendamento)    
                   .HasColumnName("dat_agendamento")
                   .IsRequired();

            builder.Property(e => e.horaAgendamento)
                   .HasColumnName("hor_agendamento")
                   .IsRequired();

            builder.Property(e => e.status)
                   .HasColumnName("dsc_status")
                   .IsRequired();

            builder.Property(e => e.dataCriacao)
                   .HasColumnName("dat_criacao")
                   .IsRequired();

            builder.HasOne(e => e.paciente)
                   .WithMany(p => p.agendamentos)
                   .HasForeignKey(e => e.idPaciente)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
