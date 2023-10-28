namespace Transaction.API.Application.ModelDTOs.v1.Payment.Request.Unregistered
{
    public record Unreg_AccountPayments_Request_DTO
    {
        public int ordinal { get; init; }
        public Unreg_BillerAccount_Request_DTO billerAccount { get; init; }
        public Unreg_ServiceFeeAmount_Request_DTO serviceFeeAmount { get; init; }
        public Unreg_PrincipalAmount_Request_DTO principalAmount { get; init; }
    }
}
