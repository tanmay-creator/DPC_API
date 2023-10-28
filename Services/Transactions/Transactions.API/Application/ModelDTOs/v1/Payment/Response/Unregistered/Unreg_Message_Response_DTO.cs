namespace Transaction.API.Application.ModelDTOs.v1.Payment.Response.Unregistered
{
    public record Unreg_Message_Response_DTO
    {
        public string code { get; init; }
        public string @default { get; init; }
    }
}
