namespace Transaction.API.Application.Services.v1.Services.Abstraction
{
    public interface IUtilitiesService
    {
        Task<HttpResponseMessage> ValidateInboundRequest(string jsonRequest, string schemaName, string vendorCode, string lobCode);
        HttpResponseMessage GetApiErrorDetails(string errorCategory, string vendorCode, string lobCode);
    }
}
