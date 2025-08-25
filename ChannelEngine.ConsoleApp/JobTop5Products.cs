using ChannelEngine.Business.Models;
using ChannelEngine.Business.Services;
using Microsoft.Extensions.Hosting;

namespace ChannelEngine.ConsoleApp
{
    public class JobTop5Products(IProductService productService) : IHostedService
    {
        public async Task StartAsync(CancellationToken ct)
        {
            List<Product> top5;
            Product? productToModify;

            try
            {
                top5 = await productService.GetTop5ProductsAsync(ct);

                if (top5.Count == 0)
                {
                    Console.WriteLine("No products found.");
                    return;
                }

                productToModify = top5.First();

                Console.WriteLine("Top 5 Products:");
                PrintTable(top5);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to fetch products: {ex.Message}");
                return;
            }

            if (productToModify != null)
            {
                try
                {
                    var modifiedProduct = await productService.UpdateStockAsync(productToModify, ct);
                    if (modifiedProduct is not null)
                    {
                        Console.WriteLine(
                            $"Updated {modifiedProduct.MerchantProductNo}, GTIN {modifiedProduct.Gtin}, new stock = {modifiedProduct.StockLocation!.Stock}"
                        );
                    }
                    else
                    {
                        Console.WriteLine("Stock update failed");
                    }
                }
                catch (Exception stockEx)
                {
                    Console.WriteLine($"Stock update failed: {stockEx.Message}");
                }
            }
        }

        public Task StopAsync(CancellationToken ct) => Task.CompletedTask;

        private static void PrintTable(IEnumerable<Product> products)
        {
            Console.WriteLine("MerchantProductNo |    GTIN    |   Quantity");
            Console.WriteLine("--------------------------------------------");
            foreach (var p in products)
            {
                Console.WriteLine($"{p.MerchantProductNo}\t{p.Gtin}\t{p.TotalQuantity}");
            }
            Console.WriteLine("--------------------------------------------");
        }
    }
}
