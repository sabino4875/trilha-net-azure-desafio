﻿namespace TrilhaNetAzureDesafio.ViewModels
{
    using System;
    public class FuncionarioViewModel
    {
        public Nullable<Guid> Id { get; set; }
        public String Nome { get; set; }
        public String Endereco { get; set; }
        public String Ramal { get; set; }
        public String EmailProfissional { get; set; }
        public String Departamento { get; set; }
        public Nullable<Decimal> Salario { get; set; }
        public Nullable<DateTimeOffset> DataAdmissao { get; set; }
    }
}
