using WebBanDoAn.ViewModels.CartModels;
using WebBanDoAn.ViewModels.CommonModels;
using WebBanDoAn.ViewModels.OrderModels;

namespace WebBanDoAn.IServices
{
    public interface IPaymentServices
    {
        #region Thanh toan Online

        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

        #endregion

        // thanh toán cho User đã đăng nhập (có userId, đọc dữ liệu từ Cart + CartItem trong DB)
        // thanh toán cho User chưa đăng nhập (không userId, đọc dữ liệu từ LocalStotage(IEnum<AddToCartFromLocalModel>))

        #region Thanh toan Offline

        Task<ResponseModel<bool>> MakeCheckOutAuthen(PaymentInformationModel model);
        Task<ResponseModel<bool>> MakeCheckOutUnauthen(PaymentCreateUnauthenModel model);

        #endregion

        Task<ResponseModel<bool>> UpdateStatusPaymentOfOrder();
    }
}
