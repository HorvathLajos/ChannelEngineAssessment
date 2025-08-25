namespace ChannelEngine.Business.Models.APICommunication
{
    public class UpdateStockRequest
    {
        public string MerchantProductNo { get; set; } = null!;
        public List<StockLocation> StockLocations { get; set; } = [];
    }

    public class StockLocation
    {
        public int Id { get; set; }
        public int Stock { get; set; }
    }
}
