namespace WebBanDoAn.ViewModels.ProductModels
{
    public class ProductReviewModel
    {
        public int Id { get; set; }

        public int? ProductId { get; set; }

        public string? UserId { get; set; }
        public string? UserName { get; set; }

        public string? ContentRated { get; set; }

        public int? PointEvaluation { get; set; }

        public string? ContentSeen { get; set; }

        public int? Status { get; set; }
    }
}
