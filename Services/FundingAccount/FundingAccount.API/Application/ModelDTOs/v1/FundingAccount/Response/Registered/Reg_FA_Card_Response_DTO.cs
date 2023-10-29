using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Request;
using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Request.Registered;

namespace FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Response.Registered;
public record Reg_FA_Card_Response_DTO
{

    public string id { get; init; }
    public Reg_OwnerDetails_Response_DTO owner { get; init; }

    public string kind { get; init; }
    public string cardHolderName { get; init; }
    public string cardNumber { get; init; }
    public string expirationDate { get; init; }

    // Optional fields
    public string securityCode { get; init; }
    public string truncatedCardNumber { get; init; }
    public string cardNumberHash { get; init; }

    public string name { get; init; }
    public string paymentMethodKind { get; init; }

    public BillingAddress_Response_DTO billingAddress { get; init; }
    public bool singleUse { get; init; } = false;
}
