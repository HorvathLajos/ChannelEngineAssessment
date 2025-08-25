using ChannelEngine.Business.Models.APICommunication;

namespace ChannelEngine.Business.Models
{
    public class Product
    {
        public string MerchantProductNo { get; set; } = string.Empty;
        public string? Gtin { get; set; }
        public int TotalQuantity { get; set; }
        public StockLocation? StockLocation { get; set; }
    }
}
