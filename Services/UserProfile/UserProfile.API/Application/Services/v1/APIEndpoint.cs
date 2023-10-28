namespace UserProfile.API.Application.Services.v1
{
    public static class APIEndpoint
    {
        public static class UserProfileEndpoint
        {
            //public static string GetUserProfile(string baseUri, Guid userprofileId) => $"{baseUri}/{userprofileId}";
            public static string ValidateRequestResponse(string baseUri, string schemaName, string vendorCode, string lobCode) => $"{baseUri}/v1/validate-inbound-request/{vendorCode}/{lobCode}?schema-name={schemaName}";
            public static string CreateUserProfile(string baseUri) => $"{baseUri}/CreateUserProfile";
            public static string GetUserProfile(string baseUri, string profileId) => $"{baseUri}/GetUserProfile/{profileId}";
            public static string UpdateUserProfile(string baseUri, string profileId) => $"{baseUri}/UpdateUserProfile/{profileId}";
            public static string UpdateCommunicationPreferences(string baseUri, string profileId, string kind) => $"{baseUri}/UpdateCommunicationPreferences/{profileId}/{kind}";
            public static string GetApiErrorDetails(string baseUri, string errorCategory, string vendorCode, string lobCode) => $"{baseUri}/v1/api-error-details/{vendorCode}/api-errors/{errorCategory}/{lobCode}";


        }
    }

}
