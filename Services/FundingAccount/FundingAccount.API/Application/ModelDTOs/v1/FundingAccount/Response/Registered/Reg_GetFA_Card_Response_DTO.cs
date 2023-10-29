namespace FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Response.Registered
{
    public record Reg_GetFA_Card_Response_DTO
    {
        public string id { get; init; }
        public Reg_GetFA_Owner_Response_DTO owner { get; init; }

        public string kind { get; init; }
        public string cardHolderName { get; init; }
        //public string cardNumber { get; init; }
        public string expirationDate { get; init; }

        // Optional fields
        //public string securityCode { get; init; }
        public string truncatedCardNumber { get; init; }
        //public string cardNumberHash { get; init; }

        public string name { get; init; }
        public string paymentMethodKind { get; init; }
        public string brandKind { get; init; }

        public Reg_BillingAddress_Response_DTO billingAddress { get; init; }
        public bool singleUse { get; init; } = false;
    }
}
