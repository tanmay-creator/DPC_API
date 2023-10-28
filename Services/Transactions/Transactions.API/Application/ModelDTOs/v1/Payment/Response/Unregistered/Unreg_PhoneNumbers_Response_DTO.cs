namespace Transaction.API.Application.ModelDTOs.v1.Payment.Response.Unregistered
{
    public record Unreg_PhoneNumbers_Response_DTO
    {
        public string kind { get; init; }
        public string number { get; init; }
        public bool allowSms { get; init; }
    }
}
