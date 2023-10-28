namespace UserProfile.API.Application.ModelDTOs.v1.UserProfile.Request
{
    public record CommunicationPreference_Request_DTO
    {
        //public string kind { get; init; }
        //public string emailAddress { get; init; }
        //public bool useDefault { get; init; } = true;
        //public string[] enabledNotifications { get; init; }
        public string kind { get; set; }
        public string emailAddress { get; set; }
        public string mobileNumber { get; set; } 
        public List<string> enabledNotifications { get; set; }
        public bool useDefault { get; set; }

    }
}
