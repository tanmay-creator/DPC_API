namespace FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Response.Registered;

public record Reg_OwnerDetails_Response_DTO
{
    public string kind { get; init; }
    //public Guid userId { get; set; }
    public string profileId { get; init; }
}
