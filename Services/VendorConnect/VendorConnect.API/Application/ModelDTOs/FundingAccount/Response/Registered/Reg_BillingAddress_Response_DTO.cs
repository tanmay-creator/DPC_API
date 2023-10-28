namespace VendorConnect.API.Application.ModelDTOs.v1.FundingAccount.Response.Registered
{
    public record Reg_BillingAddress_Response_DTO
    {
        public string countryCode { get; init; }
        public List<string> lines { get; init; }
        public string city { get; init; }
        public string regionCode { get; init; }
        public string postalCode { get; init; }
    }
}
