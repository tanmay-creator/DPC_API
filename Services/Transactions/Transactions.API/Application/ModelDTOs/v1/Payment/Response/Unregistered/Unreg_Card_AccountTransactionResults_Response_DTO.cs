namespace Transaction.API.Application.ModelDTOs.v1.Payment.Response.Unregistered
{
    public record Unreg_Card_AccountTransactionResults_Response_DTO
    {
        public int ordinal { get; set; }
        public Unreg_BillerAccount_Response_DTO billerAccount { get; init; }
        public Unreg_ServiceFeeAmount_Response_DTO serviceFeeAmount { get; init; }
        public Unreg_PrincipalAmount_Response_DTO principalAmount { get; init; }
        public Unreg_CustomFields_Response_DTO[] customFields { get; init; }
        public Unreg_ProcessorResponse_Response_DTO processorResponse { get; init; }
    }
}
