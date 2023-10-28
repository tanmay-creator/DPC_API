using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Request;

namespace FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Request.Registered;

public record Reg_FA_ACH_Request_DTO
{
    public Reg_OwnerDetails_Request_DTO owner { get; init; }

    public string kind { get; init; }
    public string paymentMethodKind { get; init; }
    public string brandKind { get; init; }
    public string accountHolderName { get; init; }
    public string aba { get; init; }
    public string dda { get; init; }

    public BillingAddress_Request_DTO billingAddress { get; init; }
    public bool singleUse { get; init; } = false;
}
