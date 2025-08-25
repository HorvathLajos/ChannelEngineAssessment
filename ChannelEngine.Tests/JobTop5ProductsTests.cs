using ChannelEngine.Business.Models;
using ChannelEngine.Business.Models.APICommunication;
using ChannelEngine.Business.Services;
using ChannelEngine.ConsoleApp;
using Moq;

public class JobTop5ProductsTests
{
    private readonly Mock<IProductService> _mockService;

    public JobTop5ProductsTests()
    {
        _mockService = new Mock<IProductService>();
    }

    [Fact]
    public async Task StartAsync_PrintsTopProductsAndUpdatesStock_WhenDataIsCorrect()
    {
        var topProducts = new List<Product>
        {
            new Product { MerchantProductNo = "A", Gtin = "GTIN1", TotalQuantity = 5, StockLocation = new StockLocation { Id = 1, Stock = 10 } },
            new Product { MerchantProductNo = "B", Gtin = "GTIN2", TotalQuantity = 3, StockLocation = new StockLocation { Id = 2, Stock = 20 } }
        };

        _mockService.Setup(s => s.GetTop5ProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(topProducts);

        _mockService.Setup(s => s.UpdateStockAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken ct) => { p.StockLocation!.Stock = 25; return p; });

        var job = new JobTop5Products(_mockService.Object);

        // Redirect console output
        using var sw = new StringWriter();
        Console.SetOut(sw);

        
        await job.StartAsync(CancellationToken.None);

        
        var output = sw.ToString();
        Assert.Contains("Top 5 Products:", output);
        Assert.Contains("A", output);
        Assert.Contains("B", output);
        Assert.Contains("Updated A, GTIN GTIN1, new stock = 25", output);
    }

    [Fact]
    public async Task StartAsync_PrintsNoProducts_WhenServiceReturnsEmptyList()
    {
        _mockService.Setup(s => s.GetTop5ProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        var job = new JobTop5Products(_mockService.Object);

        using var sw = new StringWriter();
        Console.SetOut(sw);

        
        await job.StartAsync(CancellationToken.None);

        
        var output = sw.ToString();
        Assert.Contains("No products found.", output);
    }

    [Fact]
    public async Task StartAsync_HandlesGetTop5ProductsException_Gracefully()
    {
        _mockService.Setup(s => s.GetTop5ProductsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Service failure"));

        var job = new JobTop5Products(_mockService.Object);

        using var sw = new StringWriter();
        Console.SetOut(sw);

        
        await job.StartAsync(CancellationToken.None);

        
        var output = sw.ToString();
        Assert.Contains("Failed to fetch products: Service failure", output);
    }

    [Fact]
    public async Task StartAsync_HandlesUpdateStockException_Gracefully()
    {
        var topProducts = new List<Product>
        {
            new Product { MerchantProductNo = "A", Gtin = "GTIN1", TotalQuantity = 5, StockLocation = new StockLocation { Id = 1, Stock = 10 } }
        };

        _mockService.Setup(s => s.GetTop5ProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(topProducts);

        _mockService.Setup(s => s.UpdateStockAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Stock update failed"));

        var job = new JobTop5Products(_mockService.Object);

        using var sw = new StringWriter();
        Console.SetOut(sw);

        
        await job.StartAsync(CancellationToken.None);

        
        var output = sw.ToString();
        Assert.Contains("Stock update failed: Stock update failed", output);
    }
}