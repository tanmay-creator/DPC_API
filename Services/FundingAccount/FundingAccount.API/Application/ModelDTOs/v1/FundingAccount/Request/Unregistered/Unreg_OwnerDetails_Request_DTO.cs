namespace FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Request.Unregistered;

public record Unreg_OwnerDetails_Request_DTO
{
    public string kind { get; init; }
    public Guid billerId { get; init; }
    public string billerAccountId { get; init; }
}
