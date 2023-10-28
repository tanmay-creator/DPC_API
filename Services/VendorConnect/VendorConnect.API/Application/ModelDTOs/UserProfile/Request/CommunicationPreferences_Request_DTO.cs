namespace VendorConnect.API.Application.ModelDTOs.UserProfile.Request
{
    public record CommunicationPreference_Request_DTO
    {
        public string kind { get; init; }
        public string emailAddress { get; init; }
        public string mobileNumber { get; set; }
        public bool useDefault { get; init; } = true;
        public List<string> enabledNotifications { get; init; }

    }
}
