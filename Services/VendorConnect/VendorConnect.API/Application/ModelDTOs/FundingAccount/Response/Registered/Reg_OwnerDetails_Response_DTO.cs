namespace VendorConnect.API.Application.ModelDTOs.FundingAccount.Response.Registered;

public record Reg_OwnerDetails_Response_DTO
{
    public string kind { get; init; }
    public Guid userId { get; init; }
}
