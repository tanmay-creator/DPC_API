namespace FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Response
{
    public record FA_Validate_Response_DTO
    {
        public bool isValid { get; init; }
        public string kind { get; init; }
        public string paymentMethodKind { get; init; }
        public string brandKind { get; init; }
        public string truncatedCardNumber { get; init; }
    }
}
