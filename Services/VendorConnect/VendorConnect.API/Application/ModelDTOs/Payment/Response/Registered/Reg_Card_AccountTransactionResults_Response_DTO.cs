namespace VendorConnect.API.Application.ModelDTOs.Payment.Response.Registered
{
    public record Reg_Card_AccountTransactionResults_Response_DTO
    {
        public int ordinal { get; set; }
        public Reg_BillerAccount_Response_DTO billerAccount { get; init; }
        public Reg_ServiceFeeAmount_Response_DTO serviceFeeAmount { get; init; }
        public Reg_PrincipalAmount_Response_DTO principalAmount { get; init; }
        public Reg_CustomFields_Response_DTO[] customFields { get; init; }
        public Reg_ProcessorResponse_Response_DTO processorResponse { get; init; }
    }
}
