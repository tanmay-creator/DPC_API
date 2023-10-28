using Microsoft.AspNetCore.Http;

namespace VendorConnect.API.Application.ModelDTOs.v1.FundingAccount.Response.Registered
{
    public record Reg_Data_Response_DTO
    {
        public string id { get; init; }
        public string kind { get; init; }
        public string name { get; init; }
        public Reg_Owner_Response_DTO owner { get; init; }
        public Reg_BillingAddress_Response_DTO billingAddress { get; init; }
        public bool singleUse { get; init; }
        public string paymentMethodKind { get; init; }
        public string brandKind { get; init; }
        public string truncatedCardNumber { get; init; }
        public string expirationDate { get; init; }
        public string cardHolderName { get; init; }
        public string truncatedDda { get; init; } // This property is common to both Card and Ach objects
        public string accountHolderName { get; init; }
        public string aba { get; init; }
    }
}
