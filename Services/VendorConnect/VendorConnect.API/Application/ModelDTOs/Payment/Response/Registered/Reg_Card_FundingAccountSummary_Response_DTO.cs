namespace VendorConnect.API.Application.ModelDTOs.Payment.Response.Registered
{
    public record Reg_Card_FundingAccountSummary_Response_DTO
    {
        public string kind { get; init; }
        public string paymentMethodKind { get; init; }
        public string brandKind { get; init; }
        public string name { get; init; }
        public string truncatedAccountNumber { get; init; }
        public Reg_AccountHolder_Response_DTO accountHolder { get; init; }

        public string cardNumberHash { get; init; }
        public string userProfileId { get; init; }
    }
}
