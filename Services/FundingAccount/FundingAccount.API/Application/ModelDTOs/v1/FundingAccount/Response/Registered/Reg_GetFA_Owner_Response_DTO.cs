namespace FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Response.Registered
{
    public record Reg_GetFA_Owner_Response_DTO
    {
        public string kind { get; init; }
        public string profileId { get; init; }
    }
}
