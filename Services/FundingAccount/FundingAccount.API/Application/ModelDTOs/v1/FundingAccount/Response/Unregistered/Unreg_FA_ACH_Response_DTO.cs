using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Response;

namespace FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Response.Unregistered
{

    public class Unreg_FA_ACH_Response_DTO
    {
        public string id { get; set; }
        public Unreg_OwnerDetails_Response_DTO owner { get; set; }
        public string kind { get; set; }
        public string paymentMethodKind { get; set; }
        public string brandKind { get; set; }
        public string accountHolderName { get; set; }
        public string aba { get; set; }
        public string truncatedDda { get; set; }

        public BillingAddress_Response_DTO billingAddress { get; set; }
        public bool singleUse { get; set; } = true;
    }
}
