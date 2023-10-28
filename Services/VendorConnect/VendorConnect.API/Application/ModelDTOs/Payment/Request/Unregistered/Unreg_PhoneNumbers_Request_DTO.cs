namespace VendorConnect.API.Application.ModelDTOs.Payment.Request.Unregistered
{
    public record Unreg_PhoneNumbers_Request_DTO
    {
        public string kind { get; init; }
        public string number { get; init; }
        public bool allowSms { get; init; }
    }
}
