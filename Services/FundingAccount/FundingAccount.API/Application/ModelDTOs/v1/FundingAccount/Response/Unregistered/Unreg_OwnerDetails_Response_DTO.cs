namespace FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Response.Unregistered;

public class Unreg_OwnerDetails_Response_DTO
{
    public string kind { get; set; }
    public Guid billerId { get; set; }
    public string billerAccountId { get; set; }
}
