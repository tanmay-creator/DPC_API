namespace VendorConnect.API.Application.ModelDTOs.Payment.Response.Registered
{
    public record Reg_ServiceFeeAmount_Response_DTO
    {
        public long value { get; init; }
        public int precision { get; init; }
        public string currencyCode { get; init; }
    }
}
