namespace TrilhaNetAzureDesafio.TableConfigurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using System;
    using TrilhaNetAzureDesafio.Models;

    public class FuncionarioConfiguration : IEntityTypeConfiguration<Funcionario>
    {
        public void Configure(EntityTypeBuilder<Funcionario> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            builder.ToTable("Funcionarios");
            builder.HasKey(e => e.Id).HasName("PK_FUNCIONARIOS");
            builder.Property<Guid>(e => e.Id).HasColumnName("Id").IsRequired();
            builder.Property<String>(e => e.Nome).HasColumnName("Nome").HasMaxLength(50).IsRequired();
            builder.Property<String>(e => e.Endereco).HasColumnName("Endereco").HasMaxLength(100).IsRequired();
            builder.Property<String>(e => e.Ramal).HasColumnName("Ramal").HasMaxLength(30).IsRequired();
            builder.Property<String>(e => e.EmailProfissional).HasColumnName("Email_Profissional").HasMaxLength(100).IsRequired();
            builder.Property<String>(e => e.Departamento).HasColumnName("Departamento").HasMaxLength(30).IsRequired();
            builder.Property<Decimal>(e => e.Salario).HasColumnName("Salario").IsRequired().HasPrecision(15, 2);
            builder.Property<DateTimeOffset>(e => e.DataAdmissao).HasColumnName("Data_Admissao").IsRequired();
        }
    }
}
