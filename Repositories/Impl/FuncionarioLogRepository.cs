namespace TrilhaNetAzureDesafio.Repositories.Impl
{
    using Azure.Data.Tables;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using TrilhaNetAzureDesafio.Context;
    using TrilhaNetAzureDesafio.Models;

    public class FuncionarioLogRepository : IFuncionarioLogRepository
    {
        private Boolean _disposable;
        private readonly ILogger _logger;
        private readonly TableClient _context;

        public FuncionarioLogRepository(IAzureTableContext context, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            var tablename = configuration.GetValue<String>("AzureTableStorage:TableName");
            _logger = Log.ForContext<FuncionarioLogRepository>();
            _context = context.Client.GetTableClient(tablename);
            _context.CreateIfNotExists();
            _disposable = true;
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (_disposable && disposing)
            {
                _disposable = false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FuncionarioLogRepository()
        {
            Dispose(false);
        }

        public Boolean Insert(FuncionarioLog entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            try
            {
                _context.UpsertEntity(entity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Houve um erro ao tentar executar a inclusão.");
                throw;
            }
        }

        public IEnumerable<FuncionarioLog> List()
        {
            return _context.Query<FuncionarioLog>();
        }
    }
}
