namespace VendorConnect.API.Application.ModelDTOs.UserProfile.Request
{
    public record UserProfile_Request_DTO
    {
        public string id { get; init; }
        //[Required]
        public string firstName { get; init; }
        public string middleName { get; init; }
        //[Required]
        public string lastName { get; init; }
        public string nameSuffix { get; init; } 
        public string emailAddress { get; init; }
        public string defaultFundingAccountId { get; init; }
        public HomeAddress_Request_DTO homeAddress { get; init; } 
        public PhoneNumbers_Request_DTO[] phoneNumbers { get; init; }
        public List<CommunicationPreference_Request_DTO> communicationPreferences { get; set; }
        public BillerAccounts_Request_DTO[] billerAccounts { get; init; } 
        
    }
}
