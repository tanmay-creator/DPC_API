namespace VendorConnect.API.Application.ModelDTOs.UserProfile.Request
{
    public record SmsCommunicationPreference_Request_DTO
    {
        //[Required]
        public string[] kind { get; init; } 
        public string mobileNumber { get; init; } 
        public Boolean useDefault { get; init; } 
        public EnabledNotifications enabledNotifications { get; init; } 
    }   
}
