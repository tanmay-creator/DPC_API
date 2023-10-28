namespace Transaction.API.Application.ModelDTOs.v1.Payment.Response.Unregistered
{
    public record Unreg_Origination_Response_DTO
    {
        public Unreg_Originator_Response_DTO originator { get; init; }
        public string paymentChannelKind { get; init; }
        public Unreg_PaymentOption_Response_DTO paymentOption { get; init; }
    }
}
