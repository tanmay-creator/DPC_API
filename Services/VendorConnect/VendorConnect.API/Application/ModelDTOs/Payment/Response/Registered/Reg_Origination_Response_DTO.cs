namespace VendorConnect.API.Application.ModelDTOs.Payment.Response.Registered
{
    public record Reg_Origination_Response_DTO
    {
        public Reg_Originator_Response_DTO originator { get; init; }
        public string paymentChannelKind { get; init; }
        public Reg_PaymentOption_Response_DTO paymentOption { get; init; }
    }
}
