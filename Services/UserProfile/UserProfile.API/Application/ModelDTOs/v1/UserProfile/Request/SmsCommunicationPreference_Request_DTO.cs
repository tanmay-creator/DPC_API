namespace UserProfile.API.Application.ModelDTOs.v1.UserProfile.Request
{
    public record SmsCommunicationPreference_Request_DTO
    {
        //[Required]
        public string[] kind { get; init; }
        public string mobileNumber { get; init; }
        public bool useDefault { get; init; }
        public EnabledNotifications_Request_DTO enabledNotifications { get; init; }
    }
}
