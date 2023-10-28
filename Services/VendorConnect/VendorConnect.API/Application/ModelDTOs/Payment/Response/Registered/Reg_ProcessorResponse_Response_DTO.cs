namespace VendorConnect.API.Application.ModelDTOs.Payment.Response.Registered
{
    public record Reg_ProcessorResponse_Response_DTO
    {
        public string responseCode { get; init; }
        public string authCode { get; init; }
        public string traceCode { get; init; }
    }
}
