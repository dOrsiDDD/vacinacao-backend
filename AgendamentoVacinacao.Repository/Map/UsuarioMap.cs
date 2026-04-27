using AgendamentoVacinacao.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendamentoVacinacao.Repository.Map
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("tb_usuario");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .HasColumnName("id_usuario")
                   .IsRequired();

            builder.Property(e => e.nome)
                   .HasColumnName("nom_usuario")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(e => e.login)
                   .HasColumnName("lgn_usuario")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(e => e.passwordHash)
                  .HasColumnName("psw_hash")
                  .IsRequired();

            builder.Property(e => e.passwordSalt)
                  .HasColumnName("psw_salt")
                  .IsRequired();

            builder.Property(e => e.email)
                   .HasColumnName("dsc_email")
                   .IsRequired();

            builder.Property(e => e.dataCriacao)
                   .HasColumnName("dat_criacao")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(e => e.perfil)
                   .HasColumnName("id_tpperfil")
                   .IsRequired();
        }
    }
}
