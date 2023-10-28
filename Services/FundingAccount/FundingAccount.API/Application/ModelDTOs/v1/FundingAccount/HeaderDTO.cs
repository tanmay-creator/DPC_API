namespace FundingAccount.API.Application.ModelDTOs.v1.FundingAccount
{
    public class HeaderDTO
    {
        [FromHeader]
        public string VendorCode { get; set; } = string.Empty;
        [FromHeader]
        public string LobCode { get; set; } = string.Empty;
    }
}
