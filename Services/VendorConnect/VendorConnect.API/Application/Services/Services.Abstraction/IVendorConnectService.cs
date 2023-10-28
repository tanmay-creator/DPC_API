// for reg
//for unreg
using VendorConnect.API.Application.ModelDTOs.FundingAccount.Request.Unregistered;
using VendorConnect.API.Application.ModelDTOs.Payment.Request.Registered;

namespace VendorConnect.API.Application.Services.Services.Abstraction

{
    public interface IVendorConnectService
    {
        Task<HttpResponseMessage> UnregisteredCardFundingAccount(Unreg_FA_Card_Request_DTO fundingaccountDetails);
        //Task<Object> UnregisteredCardFundingAccount(Unreg_FA_Card_Request_DTO fundingaccountDetails);
        
        
        Task<HttpResponseMessage> UnregisteredACHFundingAccount(Unreg_FA_ACH_Request_DTO fundingaccountDetails);
        Task<HttpResponseMessage> RegisteredCardFundingAccount(Reg_FA_Card_Request_DTO fundingaccountDetails);
        Task<HttpResponseMessage> RegisteredACHFundingAccount(Reg_FA_ACH_Request_DTO fundingaccountDetails);
        Task<HttpResponseMessage> GetAllFundingAccount(string profileId, string vendorConnect, string lobCode);

        // Payment FLow
        Task<HttpResponseMessage> RegisteredPayment(Reg_Payment_Request_DTO paymentDetails);
        Task<HttpResponseMessage> UnregisteredPayment(Unreg_Payment_Request_DTO paymentDetails);
        
        // UserProfile Flow
        Task<HttpResponseMessage> GetUserProfile( string profileId);
        Task<HttpResponseMessage> CreateUserProfile(UserProfile_Request_DTO userProfile_Request_DTO);
        //Task<HttpResponseMessage> CreateUserProfile(UserProfile_Request_DTO userProfile_Request_DTO);
        Task<HttpResponseMessage> UpdateCommunicationPreferences(string profileId, string kind, CommunicationPreference_Request_DTO comPrefRequest);
        Task<HttpResponseMessage> UpdateUserProfile(string profileID, UserProfile_Request_DTO userProfile_Request_DTO);

        // Delete Funding Account
        Task<HttpResponseMessage> DeleteUnregisteredFundingAccount(string billerId, string billerAccountId, string fundingAccountId);
        Task<HttpResponseMessage> DeleteRegisteredFundingAccount( string profileId, string fundingAccountId);

        // Validate funding account
        Task<HttpResponseMessage> ValidateFundingAccount(ValidateFA_RequestDTO fundingAccountRequest);
    }
}
