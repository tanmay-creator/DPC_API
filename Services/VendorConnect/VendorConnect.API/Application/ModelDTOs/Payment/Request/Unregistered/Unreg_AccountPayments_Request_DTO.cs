namespace VendorConnect.API.Application.ModelDTOs.Payment.Request.Unregistered
{
    public record Unreg_AccountPayments_Request_DTO
    {
        public int ordinal { get; set; }
        public Unreg_BillerAccount_Request_DTO billerAccount { get; init; }
        public Unreg_ServiceFeeAmount_Request_DTO serviceFeeAmount { get; init; }
        public Unreg_PrincipalAmount_Request_DTO principalAmount { get; init; }
    }
}
