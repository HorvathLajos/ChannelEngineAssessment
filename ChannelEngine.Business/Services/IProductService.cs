using ChannelEngine.Business.Models;

namespace ChannelEngine.Business.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetTop5ProductsAsync(CancellationToken ct = default);
        Task<Product?> UpdateStockAsync(Product product, CancellationToken ct = default);
    }
}
