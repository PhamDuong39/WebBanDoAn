using WebBanDoAn.Entities;

namespace WebBanDoAn.ViewModels.ProductModels
{
    public class DetailProductModel
    {
        public int Id { get; set; }
        public int? ProductTypeId { get; set; }
        public string? ProductTypeName { get; set; }

        public string? NameProduct { get; set; }

        public double? Price { get; set; }

        public string? AvartarImageProduct { get; set; }

        public string? Title { get; set; }

        public int? Discount { get; set; }

        public int? NumberOfViews { get; set; }
        
        public IEnumerable<ImageProductModel>? imageProductModels { get; set; } = new List<ImageProductModel>();
        public IEnumerable<ProductReviewModel>? productReviewModels { get; set; } = new List<ProductReviewModel>();
    }
}
