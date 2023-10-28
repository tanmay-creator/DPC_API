namespace VendorConnect.API.Application.ModelDTOs.UserProfile.Response
{
    public record CommunicationPreference_Response_DTO
    {
        public string kind { get; init; }
        public string emailAddress { get; init; }
        public string mobileNumber { get; set; } 
        public bool useDefault { get; init; } = true;
        public List<string> enabledNotifications { get; init; }
        public bool enabled { get; init; }
    }   
}
