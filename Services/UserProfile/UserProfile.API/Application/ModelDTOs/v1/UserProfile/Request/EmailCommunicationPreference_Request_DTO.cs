namespace UserProfile.API.Application.ModelDTOs.v1.UserProfile.Request
{
    public record EmailCommunicationPreference_Request_DTO
    {
        //[Required]
        public string kind { get; init; }
        public string emailAddress { get; init; }
        public bool useDefault { get; init; } = true;
        public EnabledNotifications_Request_DTO enabledNotifications { get; init; }

    }
}
