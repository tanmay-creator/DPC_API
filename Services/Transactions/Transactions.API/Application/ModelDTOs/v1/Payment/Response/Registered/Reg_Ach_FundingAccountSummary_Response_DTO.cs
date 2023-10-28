namespace Transaction.API.Application.ModelDTOs.v1.Payment.Response.Registered
{
    public record Reg_Ach_FundingAccountSummary_Response_DTO
    {
        public string kind { get; init; }
        public string paymentMethodKind { get; init; }
        public string brandKind { get; init; }
        public string name { get; init; }
        public string truncatedAccountNumber { get; init; }
        public Reg_AccountHolder_Response_DTO accountHolder { get; init; }
        public string aba { get; init; } = null;
        public string bankAccountHash { get; init; } = null;
        public string userProfileId { get; init; }
    }
}
