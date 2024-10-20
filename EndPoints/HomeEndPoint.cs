namespace TrilhaNetAzureDesafio.EndPoints
{
    using System;
    public class HomeEndPoint
    {
        private readonly String _message;
        private readonly Uri _uri;
        public HomeEndPoint(String message, Uri uri)
        {
            ArgumentNullException.ThrowIfNull(message);
            ArgumentNullException.ThrowIfNull(uri);

            _message = message;
            _uri = uri;
        }

        public String Mensagem => _message;
        public Uri Documentation => _uri;
    }
}
