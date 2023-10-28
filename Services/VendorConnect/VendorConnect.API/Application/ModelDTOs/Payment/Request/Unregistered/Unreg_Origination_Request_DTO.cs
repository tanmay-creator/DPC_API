namespace VendorConnect.API.Application.ModelDTOs.Payment.Request.Unregistered
{
    public record Unreg_Origination_Request_DTO
    {
        public Unreg_Originator_Request_DTO originator { get; init; }
        public string paymentChannelKind { get; init; }
        public Unreg_PaymentOption_Request_DTO paymentOption { get; init; }
    }
}
