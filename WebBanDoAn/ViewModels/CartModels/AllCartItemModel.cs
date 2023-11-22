namespace WebBanDoAn.ViewModels.CartModels
{
    public class AllCartItemModel
    {
        public int CartId { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? AvartarImageProduct { get; set; }
        public int? Quantity { get; set; }
        public double? ProductPrice { get; set; }
        public double? SubProductPrice { get; set; }
        public double? AllPrice { get; set; }
    }
}
