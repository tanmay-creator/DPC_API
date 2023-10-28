namespace Transaction.API.Application.ModelDTOs.v1.Payment.Response.Registered
{
    public record Reg_Message_Response_DTO
    {
        public string code { get; init; }
        public string @default { get; init; }
    }
}
