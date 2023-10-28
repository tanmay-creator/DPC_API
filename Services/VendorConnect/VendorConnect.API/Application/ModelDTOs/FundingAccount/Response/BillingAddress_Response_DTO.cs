namespace VendorConnect.API.Application.ModelDTOs.FundingAccount.Response
{
    public record BillingAddress_Response_DTO
    {
        public string[] lines { get; init; }
        public string city { get; init; }
        public string regionCode { get; init; }
        public string postalCode { get; init; }
        public string countryCode { get; init; }
    }
}
