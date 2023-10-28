namespace UserProfile.API.Application.Services.v1.Services.Abstraction
{
    public interface IUtilityService
    {
        Task<HttpResponseMessage> ValidateRequestResponse(string jsonRequest, string schemaName, string vendorCode, string lobCode);
        HttpResponseMessage GetApiErrorDetails(string errorCategory, string vendorCode, string lobCode);
    }
}
