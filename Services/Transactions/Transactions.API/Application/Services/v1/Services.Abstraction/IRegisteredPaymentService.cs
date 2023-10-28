using Transaction.API.Application.ModelDTOs.v1.Payment.Request.Registered;
using Transaction.API.Application.ModelDTOs.v1.Payment.Request.Unregistered;

namespace Transaction.API.Application.Services.v1.Services.Abstraction
{
    public interface IRegisteredPaymentService
    {        
        Task<HttpResponseMessage> MakeRegisteredPayment(Reg_Payment_Request_DTO payment_req, string vendorCode, string lobCode);
    }
}
