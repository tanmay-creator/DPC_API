namespace VendorConnect.API.Application.ModelDTOs.Payment.Request.Registered
{
    public record Reg_FundingAccount_Request_DTO
    {
        public string token { get; init; }
        public string userProfileId { get; init; }

        #nullable enable
        public string? securityCode { get; init; }
    }
}
