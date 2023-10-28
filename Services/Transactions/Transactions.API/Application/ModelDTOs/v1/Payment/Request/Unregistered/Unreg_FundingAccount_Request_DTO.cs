namespace Transaction.API.Application.ModelDTOs.v1.Payment.Request.Unregistered
{
    public record Unreg_FundingAccount_Request_DTO
    {
        public string token { get; init; }

#nullable enable
        public string? securityCode { get; init; }
    }
}
