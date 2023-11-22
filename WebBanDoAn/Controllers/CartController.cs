using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Operators;
using WebBanDoAn.Extensions;
using WebBanDoAn.IServices;
using WebBanDoAn.ViewModels.CartModels;

namespace WebBanDoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartServices _cartServices;

        public CartController(ICartServices cartServices)
        {
            _cartServices = cartServices;
        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddProductToCart([FromBody]AddToCartModel addToCartModel)
        {
            var result = await _cartServices.AddToCart(addToCartModel);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("remove-1-from-cart/{productId:int}")]
        public async Task<IActionResult> RemoveOneProduct(int productId)
        {
            var result = await _cartServices.RemoveCart(productId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("add-to-cart-from-local")]
        public async Task<IActionResult> AddToCartLocal(IEnumerable<AddToCartFromLocalModel> allProductModels)
        {
            var result = await _cartServices.AddToCartFromLocal(allProductModels);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("remove-all-cart")]
        public async Task<IActionResult> RemoveAllCart()
        {
            var result = await _cartServices.RemoveAllCart();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-cart-items")]
        public async Task<IActionResult> GetAllCartItems([FromQuery]Pagination pagination)
        {
            var result = await _cartServices.GetAllCartItems(pagination);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
