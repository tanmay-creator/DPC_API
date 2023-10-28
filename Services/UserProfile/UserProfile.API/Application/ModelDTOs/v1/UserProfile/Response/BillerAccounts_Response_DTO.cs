namespace UserProfile.API.Application.ModelDTOs.v1.UserProfile.Response
{
    public record BillerAccounts_Response_DTO
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