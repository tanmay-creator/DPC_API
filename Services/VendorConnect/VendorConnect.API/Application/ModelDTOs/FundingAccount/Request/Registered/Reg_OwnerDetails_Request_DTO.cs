namespace VendorConnect.API.Application.ModelDTOs.FundingAccount.Request.Registered;

public record Reg_OwnerDetails_Request_DTO
{
    public string kind { get; init; }
    // public Guid userId { get; set; }
    public string profileId { get; init; }
}

