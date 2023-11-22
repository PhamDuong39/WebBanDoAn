using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Operators;
using WebBanDoAn.Extensions;
using WebBanDoAn.IServices;
using WebBanDoAn.Services;
using WebBanDoAn.ViewModels.Users;

namespace WebBanDoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _productServices;

        public ProductController(IProductServices productServices)
        {
            _productServices = productServices;
        }

        [HttpGet("get-products")]
        public async Task<IActionResult> GetAllProducts([FromQuery]Pagination pagination)
        {
            var result = await _productServices.GetProducts(pagination);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-product/{productId:int}")]
        public async Task<IActionResult> GetDetailProduct(int productId)
        {
            var result = await _productServices.ViewDetailProduct(productId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
