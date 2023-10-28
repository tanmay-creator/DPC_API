namespace Transaction.API.Application.ModelDTOs.v1.Payment.Request.Registered
{
    public record Reg_Payment_Request_DTO
    {
        public string id { get; init; }
        public string paymentDate { get; init; }
        public Reg_FundingAccount_Request_DTO fundingAccount { get; init; }
        public Reg_Origination_Request_DTO origination { get; init; }
        public Reg_Payer_Request_DTO payer { get; init; }

        public Reg_AccountPayments_Request_DTO[] accountPayments { get; init; }
    }
}
