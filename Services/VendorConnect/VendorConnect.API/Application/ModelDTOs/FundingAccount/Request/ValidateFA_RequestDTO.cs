namespace VendorConnect.API.Application.ModelDTOs.FundingAccount.Request
{
    public record ValidateFA_RequestDTO
    {
        public string billerId {  get; init; }
        public string kind { get; init; }
        public string paymentChannelKind { get; init; }
        public string cardNumber { get; init; }
    }
}
