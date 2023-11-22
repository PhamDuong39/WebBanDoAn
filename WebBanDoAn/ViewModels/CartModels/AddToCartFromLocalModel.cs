namespace WebBanDoAn.ViewModels.CartModels
{
    public class AddToCartFromLocalModel
    {
        public int Id { get; set; }
        public int? ProductTypeId { get; set; }
        public string? ProductTypeName { get; set; }

        public string? NameProduct { get; set; }

        public double? Price { get; set; }

        public string? AvartarImageProduct { get; set; }

        public string? Title { get; set; }

        public int? Discount { get; set; }
        public int Quantity { get; set; }
    }
}
