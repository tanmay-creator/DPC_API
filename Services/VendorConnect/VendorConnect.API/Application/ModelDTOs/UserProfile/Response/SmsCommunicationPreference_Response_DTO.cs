namespace VendorConnect.API.Application.ModelDTOs.UserProfile.Response
{
    public record SmsCommunicationPreference_Response_DTO
    {
       // [Required]
        public string[] kind { get; init; } 
        public string mobileNumber { get; init; } 
        public Boolean useDefault { get; init; } 
        public EnabledNotifications_Response_DTO enabledNotifications { get; init; } 
    }
}
