namespace VendorConnect.API.Application.ModelDTOs.FundingAccount.Response.Unregistered;

public record Unreg_FA_ACH_Response_DTO
{
    public string id { get; init; }
    public Unreg_OwnerDetails_Response_DTO owner { get; init; }
    
    public string kind { get; init; }
    public string paymentMethodKind { get; init; }
    public string brandKind { get; init; }
    public string accountHolderName { get; init; }
    public string aba { get; init; }
    public string truncatedDda { get; init; }

    public BillingAddress_Response_DTO billingAddress { get; init; }
    public bool singleUse { get; init; } = true;
}
