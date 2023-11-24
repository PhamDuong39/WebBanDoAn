using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebBanDoAn.IServices;
using WebBanDoAn.ViewModels.CartModels;
using WebBanDoAn.ViewModels.OrderModels;

namespace WebBanDoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentServices _paymentServices;

        public PaymentController(IPaymentServices paymentServices)
        {
            _paymentServices = paymentServices;
        }

        [HttpPost("vnpay-create-redirect-url")]
        public IActionResult CreatePaymentUrl(PaymentInformationModel model)
        {
            var url = _paymentServices.CreatePaymentUrl(model, HttpContext);
            return Ok(url);
        }

        [HttpGet]
        public IActionResult PaymentCallback()
        {
            var response = _paymentServices.PaymentExecute(Request.Query);

            return Ok(response);
        }

        [HttpPost("checkout-unauthen")]
        public async Task<IActionResult> CheckoutNormalUnauthen([FromBody] PaymentCreateUnauthenModel paymentCreateUnauthenModel)
        {
            var result = await _paymentServices.MakeCheckOutUnauthen(paymentCreateUnauthenModel);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpPost("checkout-authen")]
        public async Task<IActionResult> CheckoutNormalAuthen([FromBody] PaymentInformationModel model)
        {
            var result = await _paymentServices.MakeCheckOutAuthen(model);
            return result.Success ? Ok(result) : BadRequest(result);
        }


    }
}
