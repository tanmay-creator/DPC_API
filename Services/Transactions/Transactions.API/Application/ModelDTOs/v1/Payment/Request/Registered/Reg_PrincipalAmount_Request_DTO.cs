namespace Transaction.API.Application.ModelDTOs.v1.Payment.Request.Registered
{
    public record Reg_PrincipalAmount_Request_DTO
    {
        public string currencyCode { get; init; }
        public int precision { get; init; }
        public long value { get; init; }
    }
}
