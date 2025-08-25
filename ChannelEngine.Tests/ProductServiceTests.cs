using ChannelEngine.Business.Clients;
using ChannelEngine.Business.Models;
using ChannelEngine.Business.Models.APICommunication;
using ChannelEngine.Business.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChannelEngine.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IChannelEngineClient> _mockClient;
        private readonly Mock<ILogger<ProductService>> _mockLogger;
        private readonly ProductService _service;
        private readonly CancellationToken _ct = default;

        public ProductServiceTests()
        {
            _mockClient = new Mock<IChannelEngineClient>();
            _mockLogger = new Mock<ILogger<ProductService>>();
            _service = new ProductService(_mockClient.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetTop5ProductsAsync_ReturnsTopProductsByOrderQuantity()
        {
            var orders = new List<Order>
            {
                new() {
                    Lines =
                    [
                        new OrderLine { MerchantProductNo = "A", Gtin="GTIN1", Quantity = 5, StockLocation = new OrderLineStockLocation{ Id = 1 } },
                        new OrderLine { MerchantProductNo = "B", Gtin="GTIN2", Quantity = 3, StockLocation = new OrderLineStockLocation{ Id = 2 } }
                    ]
                },
                new()
                {
                    Lines =
                    [
                        new OrderLine { MerchantProductNo = "A", Gtin="GTIN1", Quantity = 2, StockLocation = new OrderLineStockLocation{ Id = 1 } },
                        new OrderLine { MerchantProductNo = "C", Gtin="GTIN3", Quantity = 6, StockLocation = new OrderLineStockLocation{ Id = 3 } }
                    ]
                }
            };

            _mockClient.Setup(c => c.GetOrdersAsync("IN_PROGRESS", _ct))
                .ReturnsAsync(orders);


            var result = await _service.GetTop5ProductsAsync();

            
            Assert.Equal(3, result.Count);
            Assert.Equal("A", result[0].MerchantProductNo);
            Assert.Equal(7, result[0].TotalQuantity);
        }

        [Fact]
        public async Task UpdateStockAsync_UpdatesStockSuccessfully_WhenDataIsCorrect()
        {
            var product = new Product
            {
                MerchantProductNo = "A",
                Gtin = "GTIN1",
                StockLocation = new StockLocation { Id = 1 }
            };

            _mockClient.Setup(c => c.UpdateStockAsync(It.IsAny<UpdateStockRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SetStockApiResponse { Success = true });

            
            var updatedProduct = await _service.UpdateStockAsync(product);

            
            Assert.NotNull(updatedProduct);
            Assert.Equal(25, updatedProduct.StockLocation!.Stock);
        }

        [Fact]
        public async Task GetTop5ProductsAsync_ReturnsEmptyList_WhenNoOrders()
        {
            _mockClient.Setup(c => c.GetOrdersAsync("IN_PROGRESS", _ct))
                .ReturnsAsync(new List<Order>());

            
            var result = await _service.GetTop5ProductsAsync();

            
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTop5ProductsAsync_IgnoresNull_whenOrderWithNullLines()
        {
            var orders = new List<Order>
            {
                new Order (),
                new Order
                {
                    Lines = new List<OrderLine>
                    {
                        new OrderLine { MerchantProductNo = "X", Gtin="GTINX", Quantity=1, StockLocation = new OrderLineStockLocation{ Id=1 } }
                    }
                }
            };

            _mockClient.Setup(c => c.GetOrdersAsync("IN_PROGRESS", _ct))
                .ReturnsAsync(orders);

            
            var result = await _service.GetTop5ProductsAsync();

            
            Assert.Single(result);
            Assert.Equal("X", result[0].MerchantProductNo);
        }

        [Fact]
        public async Task UpdateStockAsync_ThrowsArgumentNullException_WhenProductIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateStockAsync(null!));
        }
    }
}
