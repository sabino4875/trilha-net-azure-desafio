namespace TrilhaNetAzureDesafio.Context
{
    using Azure.Data.Tables;
    using System;

    public interface IAzureTableContext : IDisposable
    {
        TableServiceClient Client { get; }
    }

    public class AzureTableContext : IAzureTableContext
    {
        private readonly TableServiceClient _client;
        private Boolean _disposable;

        public AzureTableContext(TableServiceClient client)
        {
            ArgumentNullException.ThrowIfNull(client, nameof(client));
            _client = client;
        }

        public TableServiceClient Client => _client;

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

        ~AzureTableContext()
        {
            Dispose(false);
        }
    }
}
