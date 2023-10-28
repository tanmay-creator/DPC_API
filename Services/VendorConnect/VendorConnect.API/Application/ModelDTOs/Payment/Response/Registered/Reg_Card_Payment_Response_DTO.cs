namespace VendorConnect.API.Application.ModelDTOs.Payment.Response.Registered
{
    public record Reg_Card_Payment_Response_DTO
    {
        public string id { get; init; }
        public string paymentDate { get; init; }
        public string actualTransactionDate { get; init; }
        public string transactionTimestamp { get; init; }
        public string transactionStatus { get; init; }
        public Reg_Message_Response_DTO message { get; init; }
        public string confirmationCode { get; init; }
        public Reg_Origination_Response_DTO origination { get; init; }

        public Reg_Payer_Response_DTO payer { get; init; }
        public Reg_Card_FundingAccountSummary_Response_DTO fundingAccountSummary { get; init; }
        public Reg_Card_AccountTransactionResults_Response_DTO[] accountTransactionResults { get; init; }
    }
}
