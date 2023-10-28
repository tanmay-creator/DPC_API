namespace Transaction.API.Application.ModelDTOs.v1.Payment.Request.Unregistered
{
    public record Unreg_Payment_Request_DTO
    {
        public string id { get; init; }
        public string paymentDate { get; init; }
        public Unreg_FundingAccount_Request_DTO fundingAccount { get; init; }
        public Unreg_Origination_Request_DTO origination { get; init; }
        public Unreg_Payer_Request_DTO payer { get; init; }

        public Unreg_AccountPayments_Request_DTO[] accountPayments { get; init; }
    }
}
