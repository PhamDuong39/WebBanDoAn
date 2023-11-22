using Microsoft.EntityFrameworkCore;
using WebBanDoAn.Context;
using WebBanDoAn.Entities;
using WebBanDoAn.Extensions;
using WebBanDoAn.IServices;
using WebBanDoAn.ViewModels.CommonModels;
using WebBanDoAn.ViewModels.ProductModels;

namespace WebBanDoAn.Services
{
    public class ProductServices : IProductServices
    {
        private readonly WebDbContext _webDbContext;

        public ProductServices(WebDbContext webDbContext)
        {
            this._webDbContext = webDbContext;
        }

        public async Task<ResponseModel<List<AllProductModel>>> GetProducts(Pagination pagination)
        {
            var lstProduct = await _webDbContext.Products
                .Include(p => p.ProductReviews)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductType)
                .ToListAsync();


            // convert Product => AllProductModel
            var lstAllProductModels = new List<AllProductModel>();
            foreach (var item in lstProduct)
            {
                AllProductModel allProductModel = new AllProductModel();
                allProductModel.Id = item.Id;
                allProductModel.AvartarImageProduct = item.AvartarImageProduct;
                allProductModel.ProductTypeId = item.ProductTypeId;
                allProductModel.Price = item.Price;
                allProductModel.Title = item.Title;
                allProductModel.Discount = item.Discount;
                allProductModel.NameProduct = item.NameProduct;
                allProductModel.ProductTypeName = item.ProductType.NameProductType;

                lstAllProductModels.Add(allProductModel);
            }

            var lstPagination = PageResult<AllProductModel>.ToPageResult(pagination, lstAllProductModels).ToList();
            return lstProduct.Any()
                ? new ResponseModel<List<AllProductModel>>()
                {
                    Success = true,
                    Message = "Lấy danh sách sản phẩm thành công",
                    Data = lstPagination,
                    StatusCode = StatusCodes.Status200OK
                }
                : new ResponseModel<List<AllProductModel>>()
                {
                    Success = true,
                    Message = "Danh sách sản phẩm trống",
                    StatusCode = StatusCodes.Status200OK
                };
        }

        public async Task<ResponseModel<DetailProductModel>> ViewDetailProduct(int productId)
        {
            var lstProduct = await _webDbContext.Products
                .Include(p => p.ProductReviews)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductType)
                .ToListAsync();

            var findProduct = lstProduct.First(p => p.Id == productId);


            // convert Product => DetailProductModel
            DetailProductModel detailProductModel = new DetailProductModel();
            detailProductModel.Id = findProduct.Id;
            detailProductModel.ProductTypeId = findProduct.ProductTypeId;
            detailProductModel.ProductTypeName = findProduct.ProductType.NameProductType;
            detailProductModel.NameProduct = findProduct.NameProduct;
            detailProductModel.Price = findProduct.Price;
            detailProductModel.AvartarImageProduct = findProduct.AvartarImageProduct;
            detailProductModel.Title = findProduct.Title;
            detailProductModel.Discount = findProduct.Discount;
            detailProductModel.NumberOfViews = findProduct.NumberOfViews;

            var lstProductImageModels = new List<ImageProductModel>();
            foreach (var item in findProduct.ProductImages)
            {
                ImageProductModel imageProductModel = new ImageProductModel();
                imageProductModel.Id = item.Id;
                imageProductModel.ImageProduct = item.ImageProduct;
                imageProductModel.ProductId = item.ProductId;
                lstProductImageModels.Add(imageProductModel);
            }
            detailProductModel.imageProductModels = lstProductImageModels;

            // convert ProductReview => ProductReviewModel
            var lstProductReviewsModel = new List<ProductReviewModel>();
            foreach (var item in findProduct.ProductReviews)
            {
                ProductReviewModel productReviewModel = new ProductReviewModel();
                productReviewModel.Id = item.Id;
                productReviewModel.ProductId = item.ProductId;
                productReviewModel.UserId = item.UserId;
                productReviewModel.ContentRated = item.ContentRated;
                productReviewModel.PointEvaluation = item.PointEvaluation;
                productReviewModel.ContentSeen = item.ContentSeen;
                productReviewModel.Status = item.Status;

                var user = await _webDbContext.Users.FirstOrDefaultAsync(p => p.Id == item.UserId);
                productReviewModel.UserName = user.UserName;

                lstProductReviewsModel.Add(productReviewModel);
            }
            detailProductModel.productReviewModels = lstProductReviewsModel;

            return detailProductModel != null
                ? new ResponseModel<DetailProductModel>()
                {
                    Success = true,
                    Message = $"Lấy sản phẩm có Id {productId} thành công!",
                    Data = detailProductModel,
                    StatusCode = StatusCodes.Status200OK
                }
                : new ResponseModel<DetailProductModel>()
                {
                    Success = false,
                    Message = $"Lấy sản phẩm có Id {productId} thất bại!",
                    StatusCode = StatusCodes.Status400BadRequest
                };
        }
    }
}
