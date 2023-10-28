namespace VendorConnect.API.Application.ModelDTOs.Payment.Response.Registered
{
    public record Reg_PhoneNumbers_Response_DTO
    {
        public string kind { get; init; }
        public string number { get; init; }
        public bool allowSms { get; init; }
    }
}
