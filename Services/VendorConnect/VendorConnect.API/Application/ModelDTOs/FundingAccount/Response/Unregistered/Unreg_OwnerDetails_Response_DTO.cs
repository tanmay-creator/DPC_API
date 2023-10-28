namespace VendorConnect.API.Application.ModelDTOs.FundingAccount.Response.Unregistered;

public record Unreg_OwnerDetails_Response_DTO
{
    public string kind { get; init; }
    public Guid billerId { get; init; }
    public string billerAccountId { get; init; }
}
