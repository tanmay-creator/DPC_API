using System.ComponentModel.DataAnnotations;

namespace UserProfile.API.Domain.Entities
{
    public class EmailCommunicationPreference
    {
        [Required]
        public string kind { get; set; } 
        public string emailAddress { get; set; }
        public bool useDefault { get; set; } = true;
        public EnabledNotifications enabledNotifications { get; set; }
        
    }
}
