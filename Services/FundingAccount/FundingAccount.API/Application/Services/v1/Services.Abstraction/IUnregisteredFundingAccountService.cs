using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Request.Unregistered;

namespace FundingAccount.API.Services.v1.Services.Abstraction
{
    public interface IUnregisteredFundingAccountService
    {
        //  Create FA Unregistered Flow
        Task<HttpResponseMessage> UnregisteredCardFundingAccount<T>(Unreg_FA_Card_Request_DTO fundingaccountDetails, string vendorCode, string lobCode);
        Task<HttpResponseMessage> UnregisteredACHFundingAccount<T>(Unreg_FA_ACH_Request_DTO fundingaccountDetails, string vendorCode, string lobCode);

        // Delete Funding account
        Task<HttpResponseMessage> DeleteFundingAccount_UnregisteredUser<T>(string billerId, string billerAccountId, string fundingAccountId, string vendorCode, string lobCode);        


    }
}
