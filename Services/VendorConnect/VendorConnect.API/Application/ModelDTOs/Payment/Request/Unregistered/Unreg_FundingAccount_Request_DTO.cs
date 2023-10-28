namespace VendorConnect.API.Application.ModelDTOs.Payment.Request.Unregistered
{
    public record Unreg_FundingAccount_Request_DTO
    {
        public string token { get; init; }
        public string securityCode { get; init; }
    }
}
