using ChannelEngine.Business.Models;
using ChannelEngine.Business.Models.APICommunication;
using ChannelEngine.Business.Models.ViewModels;
using ChannelEngine.Business.Services;
using ChannelEngine.WebApp.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

public class ProductControllerTests
{
    private readonly Mock<IProductService> _mockService;

    public ProductControllerTests()
    {
        _mockService = new Mock<IProductService>();
    }

    private ProductController CreateControllerWithTempData()
    {
        var controller = new ProductController(_mockService.Object);

        var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        controller.TempData = tempData;

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        return controller;
    }

    [Fact]
    public async Task Index_ReturnsViewWithProductsAndUpdatedProduct_WhenDataIsCorrect()
    {
        var topProducts = new List<Product>
        {
            new() { MerchantProductNo = "A", Gtin = "GTIN1", StockLocation = new StockLocation { Id = 1, Stock = 10 } },
            new() { MerchantProductNo = "B", Gtin = "GTIN2", StockLocation = new StockLocation { Id = 2, Stock = 15 } }
        };

        _mockService.Setup(s => s.GetTop5ProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(topProducts);

        _mockService.Setup(s => s.UpdateStockAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken ct) => { p.StockLocation!.Stock = 25; return p; });

        var controller = CreateControllerWithTempData();

        
        var result = await controller.Index() as ViewResult;
        var model = result?.Model as ProductsViewModel;

        
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Equal(2, model.Products.Count);
        Assert.Equal("A", model.LastUpdatedProductNo);
        Assert.Equal(25, model.LastUpdatedQuantity);
        Assert.Null(controller.TempData["ErrorMessage"]);
    }

    [Fact]
    public async Task Index_SetsTempDataErrorMessage_WhenGetTop5ProductsFails()
    {
        _mockService.Setup(s => s.GetTop5ProductsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Service failure"));

        var controller = CreateControllerWithTempData();

        
        var result = await controller.Index() as ViewResult;

        
        Assert.NotNull(result);
        var model = result.Model as ProductsViewModel;
        Assert.NotNull(model);
        Assert.Empty(model.Products);
    }

    [Fact]
    public async Task Index_SetsTempDataErrorMessageButReturnsProducts_WhenUpdateStockFails()
    {
        var topProducts = new List<Product>
        {
            new Product { MerchantProductNo = "A", Gtin = "GTIN1", StockLocation = new StockLocation { Id = 1, Stock = 10 } }
        };

        _mockService.Setup(s => s.GetTop5ProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(topProducts);

        _mockService.Setup(s => s.UpdateStockAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Stock update failed"));

        var controller = CreateControllerWithTempData();

        
        var result = await controller.Index() as ViewResult;
        var model = result?.Model as ProductsViewModel;

        
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Single(model.Products);
        Assert.Null(model.LastUpdatedProductNo);
    }

    [Fact]
    public async Task Index_HandlesGracefully_WhenNoProductsReturned()
    {
        _mockService.Setup(s => s.GetTop5ProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        var controller = CreateControllerWithTempData();

        
        var result = await controller.Index() as ViewResult;
        var model = result?.Model as ProductsViewModel;

        
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Empty(model.Products);
        Assert.Null(model.LastUpdatedProductNo);
        Assert.Null(controller.TempData["ErrorMessage"]);
    }
}
