namespace VendorConnect.API.Application.ModelDTOs.FundingAccount.Response
{
    public record ValidateFA_ResponseDTO
    {
        public Boolean isValid { get; init; }
        public string kind { get; init; }
        public string paymentMethodKind { get; init; } 
        public string brandKind { get; init; }
        public string truncatedCardNumber { get; init; }
    }
}
