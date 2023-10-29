using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Request;

namespace FundingAccount.API.Services.v1.Services.Abstraction
{
    public interface IRegisteredFundingAccountService
    {
       
        // Create FA Registered Flow
        Task<HttpResponseMessage> RegisteredCardFundingAccount<T>(Reg_FA_Card_Request_DTO fundingAccount, string vendorCode, string lobCode);
        Task<HttpResponseMessage> RegisteredACHFundingAccount<T>(Reg_FA_ACH_Request_DTO fundingaccountDetails, string vendorCode, string lobCode);
      
        Task<HttpResponseMessage> DeleteFundingAccount_RegisteredUser<T>(string profileId, string fundingAccountId, string vendorCode, string lobCode);
        Task<HttpResponseMessage> GetAllFundingAccount(string profileId, string vendorCode, string lobCode);
        Task<HttpResponseMessage> GetFundingAccount(string profileId, string fundingAccountId, string vendorCode, string lobCode);


    }
}
