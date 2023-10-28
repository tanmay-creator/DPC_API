namespace Transaction.API.Application.ModelDTOs.v1.Payment.Request.Registered
{
    public record Reg_BillerAccount_Request_DTO
    {
        public string billerAccountId { get; init; }
        public string billerId { get; init; }
    }
}
