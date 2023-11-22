using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebBanDoAn.Context;
using WebBanDoAn.Entities;
using WebBanDoAn.Extensions;
using WebBanDoAn.IServices;
using WebBanDoAn.ViewModels.CartModels;
using WebBanDoAn.ViewModels.CommonModels;
using WebBanDoAn.ViewModels.ProductModels;

namespace WebBanDoAn.Services
{
    public class CartServices : ICartServices
    {
        private readonly WebDbContext _webDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartServices(WebDbContext webDbContext, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _webDbContext = webDbContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Add / Remove one to cart

        public async Task<ResponseModel<bool>> AddToCart(AddToCartModel addToCartModel)
        {
            string userId = GetUserId();
            using (var trans = _webDbContext.Database.BeginTransaction())
            {
                try
                {
                    if (string.IsNullOrEmpty(userId))
                    {
                        return new ResponseModel<bool>()
                        {
                            Success = false,
                            Message = "Người dùng chưa đăng nhập",
                            StatusCode = StatusCodes.Status401Unauthorized
                        };
                    }
                    // cart
                    var cart = await GetCart(userId);
                    if (cart is null)
                    {
                        cart = new Cart();
                        cart.UserId = userId;
                        cart.CreatedAt = DateTime.Now;
                        _webDbContext.Carts.Add(cart);
                    }
                    _webDbContext.SaveChanges();

                    // cartItem
                    var cartItem = _webDbContext.CartItems.FirstOrDefault(p => p.CartId == cart.Id && p.ProductId == addToCartModel.ProductId);
                    if (cartItem is not null)
                    {
                        cartItem.Quantity += addToCartModel.quantity;
                        cartItem.UpdateAt = DateTime.Now;
                        _webDbContext.CartItems.Update(cartItem);
                    }
                    else
                    {
                        cartItem = new CartItem();
                        cartItem.ProductId = addToCartModel.ProductId;
                        cartItem.CartId = cart.Id;
                        cartItem.Quantity = addToCartModel.quantity;
                        cartItem.CreatedAt = DateTime.UtcNow;
                        _webDbContext.CartItems.Add(cartItem);
                    }
                    _webDbContext.SaveChanges();
                    trans.Commit();
                    return new ResponseModel<bool>()
                    {
                        Success = true,
                        Message = $"Thêm vào giỏ hàng sản phẩm {addToCartModel.ProductId} thành công",
                        StatusCode = StatusCodes.Status200OK
                    };
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    return new ResponseModel<bool>()
                    {
                        Success = false,
                        Message = $"Thêm vào giỏ hàng thất bại {e}",
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                }
            }
        }

        public async Task<ResponseModel<bool>> RemoveCart(int productId)
        {
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return new ResponseModel<bool>()
                    {
                        Success = false,
                        Message = "Người dùng chưa đăng nhập",
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }
                var cart = await GetCart(userId);
                if (cart is null)
                {
                    return new ResponseModel<bool>()
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin Cart của người dùng",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                // cartItem
                var cartItem = _webDbContext.CartItems.FirstOrDefault(p => p.CartId == cart.Id && p.ProductId == productId);
                if (cartItem is null)
                {
                    return new ResponseModel<bool>()
                    {
                        Success = false,
                        Message = $"Không tìm thấy sản phẩm Id {productId} trong giỏ hàng",
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                else if (cartItem.Quantity == 1)
                {
                    _webDbContext.CartItems.Remove(cartItem);
                    _webDbContext.SaveChanges();
                    return new ResponseModel<bool>()
                    {
                        Success = true,
                        Message = $"Đã xóa sản phẩm Id {productId} trong giỏ hàng",
                        StatusCode = StatusCodes.Status200OK
                    };
                }
                else
                {
                    cartItem.Quantity -= 1;
                    _webDbContext.CartItems.Update(cartItem);
                    _webDbContext.SaveChanges();
                    return new ResponseModel<bool>()
                    {
                        Success = true,
                        Message = $"Đã trừ sl 1 sản phẩm Id {productId} trong giỏ hàng",
                        StatusCode = StatusCodes.Status200OK
                    };
                }
            }
            catch (Exception e)
            {
                return new ResponseModel<bool>()
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi trong quá trình xóa giỏ hàng {e}",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        #endregion

        #region Add from Local Storage

        public async Task<ResponseModel<bool>> AddToCartFromLocal(IEnumerable<AddToCartFromLocalModel> allProductModels)
        {
            string userId = GetUserId();
            using (var trans = _webDbContext.Database.BeginTransaction())
            {
                try
                {
                    if (string.IsNullOrEmpty(userId))
                    {
                        return new ResponseModel<bool>()
                        {
                            Success = false,
                            Message = "Người dùng chưa đăng nhập",
                            StatusCode = StatusCodes.Status401Unauthorized
                        };
                    }
                    // cart
                    var cart = await GetCart(userId);
                    if (cart is null)
                    {
                        return new ResponseModel<bool>()
                        {
                            Success = false,
                            Message = "Không tìm thấy thông tin Cart của người dùng",
                            StatusCode = StatusCodes.Status404NotFound
                        };
                    }
                    var lstCartItemNews = new List<CartItem>();
                    foreach (var product in allProductModels)
                    {
                        // Cai nao ton tai roi thi ++ so luong
                        // cai nao chua co thi them moi
                        var cartItem = _webDbContext.CartItems.FirstOrDefault(p => p.CartId == cart.Id && p.ProductId == product.Id);
                        if (cartItem is not null)
                        {
                            cartItem.Quantity += product.Quantity;
                            cartItem.UpdateAt = DateTime.Now;
                            _webDbContext.CartItems.Update(cartItem);
                        }
                        else
                        {
                            cartItem = new CartItem();
                            cartItem.Id = cart.Id;
                            cartItem.ProductId = product.Id;
                            cartItem.Quantity = product.Quantity;
                            cartItem.CreatedAt = DateTime.UtcNow;
                            lstCartItemNews.Add(cartItem);
                        }
                    }
                    _webDbContext.CartItems.AddRange(lstCartItemNews);
                    _webDbContext.SaveChanges();
                    trans.Commit();

                    return new ResponseModel<bool>()
                    {
                        Success = true,
                        Message = "Lưu danh sách sản phẩm thành công",
                        StatusCode = StatusCodes.Status200OK
                    };
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    return new ResponseModel<bool>()
                    {
                        Success = false,
                        Message = $"Lưu danh sách sản phẩm thất bại {e}",
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                }
            }
        }

        #endregion

        #region Remove all cart

        public async Task<ResponseModel<bool>> RemoveAllCart()
        {
            string userId = GetUserId();
            using (var trans = _webDbContext.Database.BeginTransaction())
            {
                try
                {
                    if (string.IsNullOrEmpty(userId))
                    {
                        return new ResponseModel<bool>()
                        {
                            Success = false,
                            Message = "Người dùng chưa đăng nhập",
                            StatusCode = StatusCodes.Status401Unauthorized
                        };
                    }
                    // cart
                    var cart = await GetCart(userId);
                    if (cart is null)
                    {
                        return new ResponseModel<bool>()
                        {
                            Success = false,
                            Message = "Không tìm thấy thông tin Cart của người dùng",
                            StatusCode = StatusCodes.Status404NotFound
                        };
                    }

                    // cartItem
                    var cartItemsOfUser = _webDbContext.CartItems.Where(p => p.CartId == cart.Id);
                    _webDbContext.CartItems.RemoveRange(cartItemsOfUser);
                    _webDbContext.SaveChanges();
                    trans.Commit();
                    return new ResponseModel<bool>()
                    {
                        Success = true,
                        Message = $"Đã xóa toàn bộ sản phẩm trong giỏ hàng",
                        StatusCode = StatusCodes.Status200OK
                    };
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    return new ResponseModel<bool>()
                    {
                        Success = false,
                        Message = $"Đã xảy ra lỗi trong quá trình xóa giỏ hàng {e}",
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                }
            }
        }

        #endregion

        #region Get All cart items

        public async Task<ResponseModel<IEnumerable<AllCartItemModel>>> GetAllCartItems(Pagination pagination)
        {
            string userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return new ResponseModel<IEnumerable<AllCartItemModel>>()
                {
                    Success = false,
                    Message = "Người dùng chưa đăng nhập",
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }

            var cart = await GetCart(userId);
            if (cart is null)
            {
                return new ResponseModel<IEnumerable<AllCartItemModel>>()
                {
                    Success = false,
                    Message = "Không tìm thấy thông tin Cart của người dùng",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            var cartItemsOfUser = _webDbContext.CartItems.Include(p => p.Product).Where(p => p.CartId == cart.Id);
            var lstAllCartItemModels = new List<AllCartItemModel>();
            foreach (var cartItem in cartItemsOfUser)
            {
                AllCartItemModel allCartItemModel = new AllCartItemModel();
                allCartItemModel.CartId = cart.Id;
                allCartItemModel.ProductId = cartItem.ProductId;
                allCartItemModel.ProductName = cartItem.Product.NameProduct;
                allCartItemModel.AvartarImageProduct = cartItem.Product.AvartarImageProduct;
                allCartItemModel.Quantity = cartItem.Quantity;
                allCartItemModel.ProductPrice = cartItem.Product.Price;
                allCartItemModel.SubProductPrice = cartItem.Quantity * cartItem.Product.Price;

                allCartItemModel.AllPrice += cartItem.Quantity * cartItem.Product.Price;

                lstAllCartItemModels.Add(allCartItemModel);
                
            }

            var lstPagination = PageResult<AllCartItemModel>.ToPageResult(pagination, lstAllCartItemModels);

            return lstPagination.Any()
                ? new ResponseModel<IEnumerable<AllCartItemModel>>()
                {
                    Success = true,
                    Message = "Lấy danh sách sản phẩm trong giỏ hàng thành công",
                    Data = lstPagination,
                    StatusCode = StatusCodes.Status200OK
                }
                : new ResponseModel<IEnumerable<AllCartItemModel>>()
                {
                    Success = false,
                    Message = "Danh sách sản phẩm trong giỏ hàng trống",
                    StatusCode = StatusCodes.Status404NotFound
                };
        }


        #endregion

        #region Get Cart + User

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
    }
}
