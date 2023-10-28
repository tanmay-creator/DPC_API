namespace FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Request
{
    public record FA_Validate_Request_DTO
    {
        public string billerId { get; init; }
        public string kind { get; init; }
        public string paymentChannelKind { get; init; }
        public string cardNumber { get; init; }
    }
}
