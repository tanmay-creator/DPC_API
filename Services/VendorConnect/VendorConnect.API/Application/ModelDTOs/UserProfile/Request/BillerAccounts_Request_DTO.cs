namespace VendorConnect.API.Application.ModelDTOs.UserProfile.Request
{
    public record BillerAccounts_Request_DTO
    {
        //[Required]
        public string kind { get; init; }
        //[Required]
        public string billerId { get; init; } 
        //[Required]
        public string billerAccountId { get; init; } 
        public string status { get; init; }
    }
}