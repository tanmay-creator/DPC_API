namespace Transaction.API.Application.ModelDTOs.v1.Payment.Response.Unregistered
{
    public record Unreg_BillerAccount_Response_DTO
    {
        public string billerId { get; init; }
        public string billerAccountId { get; init; }
    }
}
