namespace TrilhaNetAzureDesafio.Context
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using TrilhaNetAzureDesafio.Models;
    using TrilhaNetAzureDesafio.TableConfigurations;

    public class RHContext(DbContextOptions<RHContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration<Funcionario>(new FuncionarioConfiguration());
        }

        public DbSet<Funcionario> Funcionarios { get; set; }
    }
}
