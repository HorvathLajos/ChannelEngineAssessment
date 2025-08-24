namespace ChannelEngine.Business.Models
{
    public class ChannelEngineApiException : Exception
    {
        public ChannelEngineApiException(string message) : base(message) { }

        public ChannelEngineApiException(string message, Exception inner) : base(message, inner) { }
    }
}
