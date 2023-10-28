namespace Transaction.API.Application.ModelDTOs.v1.Payment.Request.Registered
{
    public record Reg_Origination_Request_DTO
    {
        public Reg_Originator_Request_DTO originator { get; init; }
        public string paymentChannelKind { get; init; }
        public Reg_PaymentOption_Request_DTO paymentOption { get; init; }
    }
}
