namespace Transaction.API.Application.ModelDTOs.v1.Payment.Request.Unregistered
{
    public record Unreg_BillerAccount_Request_DTO
    {
        public string billerAccountId { get; init; }
        public string billerId { get; init; }
    }
}
