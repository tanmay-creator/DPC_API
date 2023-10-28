namespace VendorConnect.API.Application.ModelDTOs.FundingAccount.Request.Unregistered;

public record Unreg_FA_ACH_Request_DTO
{
    public Unreg_OwnerDetails_Request_DTO owner { get; init; }

    public string kind { get; init; }
    public string paymentMethodKind { get; init; }
    public string brandKind { get; init; }
    public string accountHolderName { get; init; }
    public string aba { get; init; }
    public string dda { get; init; }

    public BillingAddress_Request_DTO billingAddress { get; init; }
    public bool singleUse { get; init; } = true;
}
