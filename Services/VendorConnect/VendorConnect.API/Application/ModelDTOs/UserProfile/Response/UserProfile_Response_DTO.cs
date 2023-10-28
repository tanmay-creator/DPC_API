namespace VendorConnect.API.Application.ModelDTOs.UserProfile.Response
{
    public record UserProfile_Response_DTO
    {
        public string id { get; init; }
        public string firstName { get; init; }
        public string middleName { get; init; }
        public string lastName { get; init; }
        public string nameSuffix { get; init; } 
        public string emailAddress { get; init; }
        public string defaultFundingAccountId { get; init; }
        public HomeAddress_Response_DTO homeAddress { get; init; } 
        public PhoneNumbers_Response_DTO[] phoneNumbers { get; init; }
        public List<CommunicationPreference_Response_DTO> CommunicationPreferences { get; set; }
        public BillerAccounts_Response_DTO[] billerAccounts { get; init; } 
        public string [] authorizedUsers { get; init; }
        
    }
}
