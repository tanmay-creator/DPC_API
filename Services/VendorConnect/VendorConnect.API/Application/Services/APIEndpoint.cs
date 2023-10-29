
namespace VendorConnect.API.Application.Services
{
    public static class APIEndpoint
    {

        public static class UtilitiesEndpoint
        {
            public static string ValidateRequestResponse(string baseUri, string schemaName, string vendorCode, string lobCode) => $"{baseUri}/v1/validate-inbound-request/{vendorCode}/{lobCode}?schema-name={schemaName}";
            public static string GetApiErrorDetails(string baseUri, string errorCategory, string vendorCode, string lobCode) => $"{baseUri}/v1/api-error-details/{vendorCode}/api-errors/{errorCategory}/{lobCode}";
            
            public static string GetAciValidationInDpcFormat(string baseUri,string vendorCode, string lobCode) => $"{baseUri}/v1/validation-error-list/vendor/{vendorCode}/{lobCode}";
        }
        public static class FundingAccountEndpoint
        {
            public static string GetFundingAccount(string baseUri, Guid fundingaccountId) => $"{baseUri}/{fundingaccountId}";

            public static string CreateFundingAccount(string baseUri) => $"{baseUri}/fundingaccounts";
            public static string GetAllFundingAccountUrl(string baseUri,string profileId) => $"{baseUri}/profiles/{profileId}/fundingaccounts";
            public static string GetFundingAccountUrl(string baseUri, string profileId, string fundingAccountId) => $"{baseUri}/profiles/{profileId}/fundingaccounts/{fundingAccountId}";

            // Delete unregistered funding account
            public static string DeleteUnregFundingAccount(string baseUri, string billerId, string billerAccountId, string fundingAccountId) => $"{baseUri}/billeraccounts/{billerId}/{billerAccountId}/fundingaccounts/{fundingAccountId}";
            public static string DeleteRegFundingAccount(string baseUri, string profileId, string fundingAccountId) => $"{baseUri}/profiles/{profileId}/fundingaccounts/{fundingAccountId}";
            // Validate funding account
            public static string ValidateFundingAccount(string baseUri) => $"{baseUri}/fundingaccounts/validate";
        }

        public static class UserProfileEndpoint
        {
            // User Profile Flow
            public static string CreateUserProfile(string baseUri) => $"{baseUri}";
            public static string GetUserProfile(string baseUri, string profileId) => $"{baseUri}/{profileId}";
            public static string UpdateUserProfile(string baseUri, string profileId) => $"{baseUri}/{profileId}";
            public static string UpdateCommunicationPreferences(string baseUri, string profileId, string kind) => $"{baseUri}/{profileId}/communication-preferences/{kind}";
        }
            

        public static class PaymentEndpoint
        {
            public static string MakePayment(string baseUri) => $"{baseUri}/payments";
        }


    }
}
