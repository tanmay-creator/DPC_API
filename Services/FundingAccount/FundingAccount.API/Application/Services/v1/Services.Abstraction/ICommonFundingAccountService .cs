using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Request;

namespace FundingAccount.API.Services.v1.Services.Abstraction
{
    public interface ICommonFundingAccountService
    {
              
        Task<HttpResponseMessage> ValidateFundingAccount<T>(FA_Validate_Request_DTO fundingaccountRequest, string vendorCode, string lobCode);
    }
}
