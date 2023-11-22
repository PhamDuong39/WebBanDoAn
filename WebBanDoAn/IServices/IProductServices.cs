using Microsoft.AspNetCore.Mvc.RazorPages;
using WebBanDoAn.Entities;
using WebBanDoAn.Extensions;
using WebBanDoAn.ViewModels.CommonModels;
using WebBanDoAn.ViewModels.ProductModels;

namespace WebBanDoAn.IServices
{
    public interface IProductServices
    {
        Task<ResponseModel<List<AllProductModel>>> GetProducts(Pagination pagination);
        Task<ResponseModel<DetailProductModel>> ViewDetailProduct(int productId);
        
    }
}
