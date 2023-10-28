namespace VendorConnect.API.Application.ModelDTOs.Payment.Request.Registered
{
    public record Reg_PhoneNumbers_Request_DTO
    {
        public string kind { get; init; }
        public string number { get; init; }
        public bool allowSms { get; init; }
    }
}
