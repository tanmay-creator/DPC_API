namespace VendorConnect.API.Application.ModelDTOs.Payment.Response.Registered
{
    public record Reg_Address_Response_DTO
    {
        public string city { get; init; }
        public string regionCode { get; init; }
        public string postalCode { get; init; }
        public string countryCode { get; init; }
    }
}
