using Microsoft.AspNetCore.Mvc;
using ChannelEngine.Business.Models;
using ChannelEngine.Business.Services;
using ChannelEngine.Business.Models.ViewModels;

namespace ChannelEngine.WebApp.Controllers
{
    public class ProductController(IProductService productService) : Controller
    {
        [HttpGet("/")]
        public async Task<IActionResult> Index()
        {
            List<Product> top5;
            Product? productToModify;
            var model = new ProductsViewModel();

            try
            {
                top5 = await productService.GetTop5ProductsAsync(HttpContext.RequestAborted);
                productToModify = top5.FirstOrDefault();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to fetch products: {ex.Message}";
                return View(model);
            }

            if (productToModify != null)
            {
                try
                {
                    var modifiedProduct = await productService.UpdateStockAsync(productToModify, HttpContext.RequestAborted);
                    if (modifiedProduct is not null)
                    {
                        model.LastUpdatedProductNo = modifiedProduct.MerchantProductNo;
                        model.LastUpdatedProductGtin = modifiedProduct.Gtin;
                        model.LastUpdatedQuantity = modifiedProduct.StockLocation!.Stock;
                    }
                }
                catch (Exception stockEx)
                {
                    TempData["ErrorMessage"] = $"{stockEx.Message}";
                }
            }

            model.Products = top5;
            
            return View(model);
        }
    }
}
