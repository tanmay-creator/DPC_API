using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Request;

namespace FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Request.Unregistered;


public record Unreg_FA_Card_Request_DTO
{
    public Unreg_OwnerDetails_Request_DTO owner { get; init; }
    public string kind { get; init; }
    public string cardHolderName { get; init; }
    public string cardNumber { get; init; }
    public string expirationDate { get; init; }
    public string securityCode { get; init; }

    public BillingAddress_Request_DTO billingAddress { get; init; }
    public bool singleUse { get; init; } = true;
}
