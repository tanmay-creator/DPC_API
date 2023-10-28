namespace VendorConnect.API.Application.ModelDTOs.Payment.Request.Registered
{
    public record Reg_AccountPayments_Request_DTO
    {
        public int ordinal { get; init; }
        public Reg_BillerAccount_Request_DTO billerAccount { get; init; }
        public Reg_ServiceFeeAmount_Request_DTO serviceFeeAmount { get; init; }
        public Reg_PrincipalAmount_Request_DTO principalAmount { get; init; }
    }
}
