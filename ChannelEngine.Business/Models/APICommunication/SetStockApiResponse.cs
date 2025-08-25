namespace ChannelEngine.Business.Models.APICommunication
{
    public class SetStockApiResponse
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? ValidationErrors { get; set; }
    }
}
