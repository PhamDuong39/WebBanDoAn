using WebBanDoAn.Entities;
using WebBanDoAn.Extensions;
using WebBanDoAn.ViewModels.CartModels;
using WebBanDoAn.ViewModels.CommonModels;
using WebBanDoAn.ViewModels.ProductModels;

namespace WebBanDoAn.IServices
{
    public interface ICartServices
    {
        Task<ResponseModel<IEnumerable<AllCartItemModel>>> GetAllCartItems(Pagination pagination);
        Task<Cart> GetCart(string userId);

        Task<ResponseModel<bool>> AddToCart(AddToCartModel addToCartModel);
        Task<ResponseModel<bool>> RemoveCart(int productId);
        Task<ResponseModel<bool>> AddToCartFromLocal(IEnumerable<AddToCartFromLocalModel> allProductModels);
        Task<ResponseModel<bool>> RemoveAllCart();
    }
}
