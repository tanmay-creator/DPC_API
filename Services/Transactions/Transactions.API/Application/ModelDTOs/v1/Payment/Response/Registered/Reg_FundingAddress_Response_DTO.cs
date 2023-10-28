namespace Transaction.API.Application.ModelDTOs.v1.Payment.Response.Registered
{
    public record Reg_FundingAddress_Response_DTO
    {
        public string[] lines { get; set; } = new string[2];
        public string city { get; init; }
        public string regionCode { get; init; }
        public string postalCode { get; init; }
        public string countryCode { get; init; }

    }
}
