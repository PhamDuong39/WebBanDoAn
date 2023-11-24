using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using System.Security.Claims;
using WebBanDoAn.Context;
using WebBanDoAn.Entities;
using WebBanDoAn.Enums.Order;
using WebBanDoAn.Extensions;
using WebBanDoAn.IServices;
using WebBanDoAn.ViewModels.CartModels;
using WebBanDoAn.ViewModels.CommonModels;
using WebBanDoAn.ViewModels.OrderModels;

namespace WebBanDoAn.Services
{
    public class PaymentServices : IPaymentServices
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly WebDbContext _webDbContext;

        public PaymentServices(IConfiguration configuration, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, WebDbContext webDbContext)
        {
            _configuration = configuration;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _webDbContext = webDbContext;
        }

        #region VnPay

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

            

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            return response;
        }

        #endregion


        #region Normal checkout

        public async Task<ResponseModel<bool>> MakeCheckOutAuthen(PaymentInformationModel model)
        {
            using(var trans = _webDbContext.Database.BeginTransaction())
            {
                string userId = GetUserId();
                var cart = await GetCart(userId);

                // Nếu user đã đăng nhập => lấy dữ liệu từ Cart
               
                if (cart is not null && userId is not null)
                {
                    var cartItems = _webDbContext.CartItems.Where(p => p.CartId == cart.Id).ToList();
                    if (cartItems.Count == 0)
                    {
                        return new ResponseModel<bool>()
                        {
                            Success = false,
                            Message = "Giỏ hàng trống",
                            StatusCode = StatusCodes.Status400BadRequest
                        };
                    }
                    Order order = new Order();

                    var lstOrderItems = order.OrderDetails;
                    order.OrderDetails = null;
                    order.PaymentId = Convert.ToInt32(PaymentMethodsEnum.THANH_TOAN_KHI_NHAN_HANG);
                    order.OriginalPrice = model.Amount;
                    order.FullName = model.Name;
                    order.Email = model.Email;
                    order.Phone = model.Phone;
                    order.Address = model.Address;
                    order.CreatedAt = DateTime.Now;
                    order.OrderStatusId = Convert.ToInt32(OrderStatusEnum.WAITING);
                    order.UserId = userId;
                    _webDbContext.Orders.Add(order);
                    _webDbContext.SaveChanges();
                        
                    foreach (var item in cartItems)
                    {
                        if (_webDbContext.Products.Any(p => p.Id == item.ProductId))
                        {
                            var product = await _webDbContext.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);

                            OrderDetail orderDetail = new OrderDetail();
                            orderDetail.OrderId = order.Id;
                            orderDetail.ProductId = item.ProductId;
                         
                            orderDetail.PriceTotal = product.Price * item.Quantity;
                            orderDetail.Quantity = item.Quantity;
                            orderDetail.CreatedAt = DateTime.Now;

                            _webDbContext.OrderDetails.Add(orderDetail);
                            _webDbContext.SaveChanges();
                        }
                        else throw new Exception($"San pham nay khong ton tai. Kiem tra lai san pham {item.Id}");
                    }

                    _webDbContext.CartItems.RemoveRange(cartItems);
                    _webDbContext.SaveChanges();

                    trans.Commit();

                    
                    return new ResponseModel<bool>()
                    {
                        Success = true,
                        Message = "Tạo hóa đơn thành công",
                        StatusCode = StatusCodes.Status200OK
                    };
                }
                trans.Rollback();
                return new ResponseModel<bool>()
                {
                    Success = false,
                    Message = "Thanh toán thất bại",
                    StatusCode = StatusCodes.Status406NotAcceptable
                };
            }
        }

        public async Task<ResponseModel<bool>> MakeCheckOutUnauthen(PaymentCreateUnauthenModel model)
        {
            using (var trans = _webDbContext.Database.BeginTransaction())
            {
                string userId = GetUserId();
                var cart = await GetCart(userId);
                // Nếu chưa đăng nhập => đọc dữ liệu từ Local Storage IEnum<AddToCartFromLocalModel>
                if (userId is null)
                {
                    Order order = new Order();
                    order.PaymentId = Convert.ToInt32(PaymentMethodsEnum.THANH_TOAN_KHI_NHAN_HANG);
                    order.OriginalPrice = 0;
                    order.FullName = model.Name;
                    order.Email = model.Email;
                    order.Phone = model.Phone;
                    order.Address = model.Address;
                    order.CreatedAt = DateTime.Now;
                    order.OrderStatusId = Convert.ToInt32(OrderStatusEnum.WAITING);
                    order.UserId = null;

                    _webDbContext.Orders.Add(order);
                    _webDbContext.SaveChanges();

                    double? amount = 0;

                    foreach (var item in model.AllProductModels)
                    {
                        if (_webDbContext.Products.Any(p => p.Id == item.Id))
                        {
                            OrderDetail orderDetail = new OrderDetail();
                            orderDetail.OrderId = order.Id;
                            orderDetail.ProductId = item.Id;
                            var product = await _webDbContext.Products.FirstOrDefaultAsync(p => p.Id == item.Id);
                            orderDetail.PriceTotal = product.Price * item.Quantity;
                            orderDetail.Quantity = item.Quantity;
                            orderDetail.CreatedAt = DateTime.Now;

                            amount += product.Price * item.Quantity;

                            _webDbContext.OrderDetails.Add(orderDetail);
                            _webDbContext.SaveChanges();
                        }
                        else throw new Exception($"San pham nay khong ton tai. Kiem tra lai san pham {item.Id}");
                    }

                    order.OriginalPrice = amount;
                    _webDbContext.Orders.Update(order);
                    _webDbContext.SaveChanges();
                    trans.Commit();

                    return new ResponseModel<bool>()
                    {
                        Success = true,
                        Message = "Tạo hóa đơn thành công",
                        StatusCode = StatusCodes.Status200OK
                    };
                }
                trans.Rollback();
                return new ResponseModel<bool>()
                {
                    Success = false,
                    Message = "Thanh toán thất bại",
                    StatusCode = StatusCodes.Status406NotAcceptable
                };
            }
        }

        public async Task<Cart> GetCart(string userId)
        {
            var cart = await _webDbContext.Carts.FirstOrDefaultAsync(p => p.UserId == userId);
            return cart;
        }

        private string GetUserId()
        {
            var claimsIdentity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;

            // Kiểm tra xem danh sách các Claim có tồn tại không và có Claim NameIdentifier không
            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            // Trả về giá trị của User Id nếu nó tồn tại
            return userIdClaim?.Value;
        }


        #endregion

        public Task<ResponseModel<bool>> UpdateStatusPaymentOfOrder()
        {
            throw new NotImplementedException();
        }

    }
}
