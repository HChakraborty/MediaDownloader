namespace Downloader.Model.exceptions
{
    public class UrlProcessingException: Exception
    {
        public UrlProcessingException(string message): base(message) { }

        public UrlProcessingException(string message, Exception innerException): base(message, innerException) { }
    }
}
