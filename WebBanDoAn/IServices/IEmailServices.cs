using WebBanDoAn.ViewModels.ResponseModel;

namespace WebBanDoAn.IServices
{
    public interface IEmailServices
    {
        void SendEmail(EmailMessageSetUpModel emailMessage);
    }
}
