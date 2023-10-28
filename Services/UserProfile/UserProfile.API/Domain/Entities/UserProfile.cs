using System.ComponentModel.DataAnnotations;

namespace UserProfile.API.Domain.Entities
{
    public class UserProfile
    {
        public string id { get; set; }
        [Required]
        public string firstName { get; set; }
        public string middleName { get; set; }
        [Required]
        public string lastName { get; set; }
        public string nameSuffix { get; set; } 
        public string emailAddress { get; set; }
        public string defaultFundingAccountId { get; set; }
        public Address homeAddress { get; set; } 
        public PhoneNumbers phoneNumbers { get; set; }
        public CommunicationPreference communicationPreference { get; set; } 
        [Required]
        public UserBillerAccount[] billerAccounts { get; set; } 
        
    }
}
