namespace VendorConnect.API.Application.Services.Services.Abstraction
{
    public interface IUtilityService
    {
       Task<HttpResponseMessage> ValidateRequestResponse (string jsonRequest, string schemaName, string vendorCode, string lobCode);
        Task<HttpResponseMessage> GetApiErrorDetails(string errorCategory, string vendorCode, string lobCode);
        Task<HttpResponseMessage> GetFormatedErrors(string jsonRequest, string vendorCode, string lobCode);
    }
}
