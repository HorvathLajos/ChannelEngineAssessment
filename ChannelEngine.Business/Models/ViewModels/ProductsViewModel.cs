namespace ChannelEngine.Business.Models.ViewModels
{
    public class ProductsViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();

        public string? LastUpdatedProductNo { get; set; }
        public string? LastUpdatedProductGtin { get; set; }
        public int? LastUpdatedQuantity { get; set; }
    }
}
