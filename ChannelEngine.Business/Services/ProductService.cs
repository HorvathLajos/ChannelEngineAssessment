using ChannelEngine.Business.Clients;
using ChannelEngine.Business.Models;
using ChannelEngine.Business.Models.APICommunication;
using Microsoft.Extensions.Logging;

namespace ChannelEngine.Business.Services
{
    public class ProductService(IChannelEngineClient client, ILogger<ProductService> logger) : IProductService
    {
        private readonly int _updatedStockValue = 25;

        /// <summary>
        /// Fetch all orders with status IN_PROGRESS and return top 5 products by quantity sold
        /// </summary>
        public async Task<List<Product>> GetTop5ProductsAsync(CancellationToken ct = default)
        {
            try
            {
                var orders = await client.GetOrdersAsync(ct: ct);

                var allProductsByOrderlines = orders
                    .SelectMany(o => o.Lines)
                    .Where(l => l.StockLocation != null)
                    .Select(l => new Product
                    {
                        MerchantProductNo = l.MerchantProductNo,
                        Gtin = l.Gtin,
                        TotalQuantity = l.Quantity,
                        StockLocation = new StockLocation() { Id = l.StockLocation!.Id }
                    });

                var top5 = allProductsByOrderlines
                    .GroupBy(p => p.MerchantProductNo)
                    .Select(g =>
                    {
                        return new Product
                        {
                            MerchantProductNo = g.Key,
                            Gtin = g.FirstOrDefault()?.Gtin,
                            TotalQuantity = g.Sum(p => p.TotalQuantity),
                            StockLocation = g.OrderByDescending(p => p.TotalQuantity).First().StockLocation
                        };
                    })
                    .OrderByDescending(p => p.TotalQuantity)
                    .Take(5)
                    .ToList();

                return top5;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch top 5 products");
                throw;
            }
        }

        /// <summary>
        /// Update stock for a specific product
        /// </summary>
        public async Task<Product?> UpdateStockAsync(Product product, CancellationToken ct = default)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(product);

                var request = new UpdateStockRequest
                {
                    MerchantProductNo = product.MerchantProductNo,
                    StockLocations =
                    [
                        new StockLocation
                        {
                            Id = product.StockLocation?.Id ?? 0,
                            Stock = _updatedStockValue
                        }
                    ]
                };

                var response = await client.UpdateStockAsync(request, ct);

                logger.LogDebug("Stock updated successfully for MerchantProductNo {MerchantProductNo}", request.MerchantProductNo);
                
                product.StockLocation!.Stock = _updatedStockValue;
                return product;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception occurred while updating stock");
                throw;
            }
        }
    }
}
