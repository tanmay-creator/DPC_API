namespace VendorConnect.API.Application.ModelDTOs.UserProfile.Request
{
    public record EmailCommunicationPreference_Request_DTO
    {
        //[Required]
        public string kind { get; init; } 
        public string emailAddress { get; init; }
        public bool useDefault { get; init; } = true;
        public EnabledNotifications enabledNotifications { get; init; }
        
    }
}
