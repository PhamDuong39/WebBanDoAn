using WebBanDoAn.ViewModels.CartModels;

namespace WebBanDoAn.ViewModels.OrderModels
{
    public class PaymentCreateUnauthenModel
    {
        public IEnumerable<AddToCartFromLocalModel>? AllProductModels { get; set; }


        public string Name { get; set; }


        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int PaymentId { get; set; }
    }
}
