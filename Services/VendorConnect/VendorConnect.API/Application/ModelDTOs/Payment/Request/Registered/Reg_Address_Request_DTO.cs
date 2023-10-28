namespace VendorConnect.API.Application.ModelDTOs.Payment.Request.Registered
{
    public record Reg_Address_Request_DTO
    {
        public string city { get; init; }
        public string regionCode { get; init; }
        public string postalCode { get; init; }
        public string countryCode { get; init; }
    }
}
