using UserProfile.API.Domain.Entities;

namespace UserProfile.API.Application.ModelDTOs.v1.UserProfile.Request
{
    public record UserProfile_Request_DTO
    {
        public string id { get; init; }
        public string firstName { get; init; }
        public string middleName { get; init; }
        public string lastName { get; init; }
        public string nameSuffix { get; init; }
        public string emailAddress { get; init; }
        public string defaultFundingAccountId { get; init; }
        public HomeAddress_Request_DTO homeAddress { get; init; }
        public PhoneNumbers_Request_DTO[] phoneNumbers { get; init; } 
        public List<CommunicationPreference_Request_DTO> CommunicationPreferences { get; set; }
        public BillerAccounts_Request_DTO[] billerAccounts { get; init; }

    }
}
