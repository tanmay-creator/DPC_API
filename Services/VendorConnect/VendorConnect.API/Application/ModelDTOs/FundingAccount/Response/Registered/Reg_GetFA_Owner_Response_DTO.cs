namespace VendorConnect.API.Application.ModelDTOs.FundingAccount.Response.Registered
{
    public record Reg_GetFA_Owner_Response_DTO
    {
        public string kind { get; init; }
        public string profileId { get; init; }
    }
}
