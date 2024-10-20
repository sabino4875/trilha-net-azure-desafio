namespace TrilhaNetAzureDesafio.Models
{
    using System;
    using System.Text.Json;
    using Azure;
    using Azure.Data.Tables;
    public sealed class FuncionarioLog : Funcionario, ITableEntity
    {
        private DateTimeOffset? _timestamp;
        public FuncionarioLog() { }

        public FuncionarioLog(Funcionario funcionario, 
                              TipoAcao tipoAcao, 
                              String partitionKey, 
                              String rowKey
        )
        {
            ArgumentNullException.ThrowIfNull(funcionario);
            ArgumentNullException.ThrowIfNull(tipoAcao);
            ArgumentNullException.ThrowIfNull(partitionKey);
            ArgumentNullException.ThrowIfNull(rowKey);

            base.Id = funcionario.Id;
            base.Nome = funcionario.Nome;
            base.Endereco = funcionario.Endereco;
            base.Ramal = funcionario.Ramal;
            base.EmailProfissional = funcionario.EmailProfissional;
            base.Departamento = funcionario.Departamento;
            base.Salario = funcionario.Salario;
            base.DataAdmissao = funcionario.DataAdmissao;
            TipoAcao = tipoAcao;
            JSON = JsonSerializer.Serialize(funcionario);
            PartitionKey = partitionKey;
            RowKey = rowKey;
            _timestamp = DateTimeOffset.UtcNow;


        }

        public TipoAcao TipoAcao { get; set; }
        public String JSON { get; set; }
        public String PartitionKey { get; set; }
        public String RowKey { get; set; }
        public ETag ETag { get; set; }
        DateTimeOffset? ITableEntity.Timestamp {  get => _timestamp; set => _timestamp = value; }
    }
}