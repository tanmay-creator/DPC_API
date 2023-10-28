namespace UserProfile.API.Application.ModelDTOs.v1.UserProfile.Response
{
    public record SmsCommunicationPreference_Response_DTO
    {
        // [Required]
        public string[] kind { get; init; }
        public string mobileNumber { get; init; }
        public bool useDefault { get; init; }
        public EnabledNotifications_Response_DTO enabledNotifications { get; init; }
    }
}
