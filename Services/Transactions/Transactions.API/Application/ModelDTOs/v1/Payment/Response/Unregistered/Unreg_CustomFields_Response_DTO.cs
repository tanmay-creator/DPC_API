namespace Transaction.API.Application.ModelDTOs.v1.Payment.Response.Unregistered
{
    public record Unreg_CustomFields_Response_DTO
    {
        public string id { get; init; }
        public string value { get; init; }
    }
}
