namespace UserProfile.API.Application.ModelDTOs.v1.UserProfile.Response
{
    public record EmailCommunicationPreference_Response_DTO
    {
        //[Required]
        public string kind { get; init; }
        public string emailAddress { get; init; }
        public bool useDefault { get; init; } = true;
        public EnabledNotifications_Response_DTO enabledNotifications { get; init; }

    }
}
