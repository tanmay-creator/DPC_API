namespace VendorConnect.API.Application.ModelDTOs.FundingAccount.Response.Unregistered
{
    public record Unreg_FA_Card_Response_DTO
    {
        public string id { get; init; }
        public Unreg_OwnerDetails_Response_DTO owner { get; init; }

        public string kind { get; init; }
        public string name { get; init; }
        public string paymentMethodKind { get; init; }
        public string brandKind { get; init; }
        public string truncatedCardNumber { get; init; }
        public string cardNumberHash { get; init; }
        public string cardHolderName { get; init; }
        public string cardNumber { get; init; }
        public string expirationDate { get; init; }
        public string securityCode { get; init; }

        public BillingAddress_Response_DTO billingAddress { get; init; }
        public bool singleUse { get; init; } = true;
    }
}
