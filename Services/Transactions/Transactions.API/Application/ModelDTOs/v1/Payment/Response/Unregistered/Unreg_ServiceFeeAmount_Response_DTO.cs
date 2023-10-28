namespace Transaction.API.Application.ModelDTOs.v1.Payment.Response.Unregistered
{
    public record Unreg_ServiceFeeAmount_Response_DTO
    {
        public long value { get; init; }
        public int precision { get; init; }
        public string currencyCode { get; init; }
    }
}
