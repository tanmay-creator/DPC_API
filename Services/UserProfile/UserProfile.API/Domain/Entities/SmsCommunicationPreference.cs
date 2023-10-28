using System.ComponentModel.DataAnnotations;

namespace UserProfile.API.Domain.Entities
{
    public class SmsCommunicationPreference
    {
        [Required]
        public string[] kind { get; set; } 
        public string mobileNumber { get; set; } 
        public Boolean useDefault { get; set; } 
        public EnabledNotifications enabledNotifications { get; set; } 
    }
}
