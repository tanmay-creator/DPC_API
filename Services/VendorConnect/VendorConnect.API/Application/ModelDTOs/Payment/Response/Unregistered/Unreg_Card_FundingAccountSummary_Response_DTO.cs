namespace VendorConnect.API.Application.ModelDTOs.Payment.Response.Unregistered
{
    public record Unreg_Card_FundingAccountSummary_Response_DTO
    {
        public string kind { get; init; }
        public string paymentMethodKind { get; init; }
        public string brandKind { get; init; }
        public string name { get; init; }
        public string truncatedAccountNumber { get; init; }
        public Unreg_AccountHolder_Response_DTO accountHolder { get; init; }

        public string cardNumberHash { get; init; }
    }
}
