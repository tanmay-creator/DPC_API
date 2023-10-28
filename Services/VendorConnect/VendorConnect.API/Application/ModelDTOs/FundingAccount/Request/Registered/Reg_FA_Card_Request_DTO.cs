namespace VendorConnect.API.Application.ModelDTOs.FundingAccount.Request.Registered;
public record Reg_FA_Card_Request_DTO
    {

        public Reg_OwnerDetails_Request_DTO owner { get; init; }

        public string kind { get; init; }
        public string cardHolderName { get; init; }
        public string cardNumber { get; init; }
        public string expirationDate { get; init; }

        // Optional fields
        public string securityCode { get; init; }

        public string name { get; init; }
        public string paymentMethodKind { get; init; }

        public BillingAddress_Request_DTO billingAddress { get; init; }
        public bool singleUse { get; init; } = false;
    }

