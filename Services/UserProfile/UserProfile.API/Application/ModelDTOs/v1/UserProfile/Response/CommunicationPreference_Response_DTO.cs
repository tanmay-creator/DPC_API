using System.ComponentModel.DataAnnotations;

namespace UserProfile.API.Application.ModelDTOs.v1.UserProfile.Response
{
    public record CommunicationPreference_Response_DTO
    {
        public string kind { get; init; }
        public string emailAddress { get; init; } 
        public string mobileNumber { get; set; } 
        public bool useDefault { get; init; } = true;
        public List<string> enabledNotifications { get; set; }
        public bool enabled { get; init; }
    }
}
