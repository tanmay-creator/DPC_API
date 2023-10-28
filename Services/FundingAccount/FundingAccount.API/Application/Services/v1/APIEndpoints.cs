namespace FundingAccount.API.Application.Services.v1
{
    public static class APIEndpoints
    {
        public static string ValidateRequestResponse(string baseUri, string schemaName, string vendorCode, string lobCode) => $"{baseUri}/v1/validate-inbound-request/{vendorCode}/{lobCode}?schema-name={schemaName}";
        public static string GetFundingAccount(string baseUri, Guid fundingaccountId) => $"{baseUri}/{fundingaccountId}";
        public static string CreateFundingAccount(string baseUri) => $"{baseUri}/";
        public static string CreateUnregCardFundingAccountVendorConnect(string baseUri) => $"{baseUri}/api/v1/unregistered/cardfundingaccount";
        public static string CreateUnregACHFundingAccountVendorConnect(string baseUri) => $"{baseUri}/api/v1/unregistered/achfundingaccount";
        public static string CreateRegCardFundingAccountVendorConnect(string baseUri) => $"{baseUri}/api/v1/registered/cardfundingaccount";
        public static string CreateRegACHFundingAccountVendorConnect(string baseUri) => $"{baseUri}/api/v1/registered/achfundingaccount";
        public static string GetAllFundingAccountVendorConnect(string baseUri,string profileId) => $"{baseUri}/api/v1/registered/{profileId}/fundingaccount";
        public static string DeleteUnregFundingAccountVendorConnect(string baseUri, string billerId, string billerAccountId, string fundingAccountId) => $"{baseUri}/{billerId}/{billerAccountId}/DeleteUnregFundingAccount/{fundingAccountId}";
        public static string DeleteRegFundingAccountVendorConnect(string baseUri, string profileId, string fundingAccountId) => $"{baseUri}/{profileId}/DeleteRegFundingAccount/{fundingAccountId}";

        public static string ValidateFundingAccountVendorConnect(string baseUri) => $"{baseUri}/ValidateFundingAccount";
        public static string GetApiErrorDetails(string baseUri, string errorCategory, string vendorCode, string lobCode) => $"{baseUri}/v1/api-error-details/{vendorCode}/api-errors/{errorCategory}/{lobCode}";

    }
}
