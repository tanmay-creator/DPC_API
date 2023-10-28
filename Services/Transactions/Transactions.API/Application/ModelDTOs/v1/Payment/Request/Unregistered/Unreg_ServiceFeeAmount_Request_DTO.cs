namespace Transaction.API.Application.ModelDTOs.v1.Payment.Request.Unregistered
{
    public record Unreg_ServiceFeeAmount_Request_DTO
    {
        public string currencyCode { get; init; }
        public int precision { get; init; }
        public long value { get; init; }
    }
}
