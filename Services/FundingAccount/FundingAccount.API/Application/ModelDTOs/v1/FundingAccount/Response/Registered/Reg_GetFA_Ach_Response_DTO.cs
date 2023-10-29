

namespace FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Response.Registered
{
    public record Reg_GetFA_Ach_Response_DTO
    {
        public string id { get; init; }
        public Reg_GetFA_Owner_Response_DTO owner { get; init; }

        public string kind { get; init; }
        public string paymentMethodKind { get; init; }
        public string brandKind { get; init; }
        public string accountHolderName { get; init; }
        public string aba { get; init; }
        //public string dda { get; init; }
        public string name { get; init; }

        // Optional fields
        public string truncatedDda { get; init; }

        public Reg_BillingAddress_Response_DTO billingAddress { get; init; }
        public bool singleUse { get; init; } = false;
    }
}
