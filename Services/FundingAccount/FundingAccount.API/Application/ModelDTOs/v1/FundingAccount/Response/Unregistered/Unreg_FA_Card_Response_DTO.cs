using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Response;

namespace FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Response.Unregistered
{
    public class Unreg_FA_Card_Response_DTO
    {
        public string id { get; set; }
        public Unreg_OwnerDetails_Response_DTO owner { get; set; }

        public string kind { get; set; }
        public string name { get; set; }
        public string paymentMethodKind { get; set; }
        public string brandKind { get; set; }
        public string truncatedCardNumber { get; set; }
        public string cardNumberHash { get; set; }
        public string cardHolderName { get; set; }
        public string cardNumber { get; set; }
        public string expirationDate { get; set; }
        public string securityCode { get; set; }

        public BillingAddress_Response_DTO billingAddress { get; set; }
        public bool singleUse { get; set; } = true;
    }
}
