namespace TrilhaNetAzureDesafio.Models
{
    using System;
    public class Funcionario : IEntity
    {
        public Funcionario() { }

        public Funcionario(Guid id, 
                           String nome, 
                           String endereco, 
                           String ramal, 
                           String emailProfissional, 
                           String departamento, 
                           Decimal salario, 
                           DateTimeOffset dataAdmissao
        )
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(nome);
            ArgumentNullException.ThrowIfNull(endereco);
            ArgumentNullException.ThrowIfNull(ramal);
            ArgumentNullException.ThrowIfNull(emailProfissional);
            ArgumentNullException.ThrowIfNull(departamento);
            ArgumentNullException.ThrowIfNull(salario);
            ArgumentNullException.ThrowIfNull(dataAdmissao);

            Id = id;
            Nome = nome;
            Endereco = endereco;
            Ramal = ramal;
            EmailProfissional = emailProfissional;
            Departamento = departamento;
            Salario = salario;
            DataAdmissao = dataAdmissao;
        }

        public Funcionario(
                   String nome,
                   String endereco,
                   String ramal,
                   String emailProfissional,
                   String departamento,
                   Decimal salario,
                   DateTimeOffset dataAdmissao
)
        {
            ArgumentNullException.ThrowIfNull(nome);
            ArgumentNullException.ThrowIfNull(endereco);
            ArgumentNullException.ThrowIfNull(ramal);
            ArgumentNullException.ThrowIfNull(emailProfissional);
            ArgumentNullException.ThrowIfNull(departamento);
            ArgumentNullException.ThrowIfNull(salario);
            ArgumentNullException.ThrowIfNull(dataAdmissao);

            Id = Guid.NewGuid();
            Nome = nome;
            Endereco = endereco;
            Ramal = ramal;
            EmailProfissional = emailProfissional;
            Departamento = departamento;
            Salario = salario;
            DataAdmissao = dataAdmissao;
        }


        public Guid Id { get; set; }
        public String Nome { get; set; }
        public String Endereco { get; set; }
        public String Ramal { get; set; }
        public String EmailProfissional { get; set; }
        public String Departamento { get; set; }
        public Decimal Salario { get; set; }
        public DateTimeOffset DataAdmissao { get; set; }
    }
}