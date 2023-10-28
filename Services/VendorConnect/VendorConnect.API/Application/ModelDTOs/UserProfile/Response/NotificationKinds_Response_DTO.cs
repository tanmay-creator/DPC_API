namespace VendorConnect.API.Application.ModelDTOs.UserProfile.Response
{
    public record  NotificationKinds_Response_DTO
    {
        enum NotificationKind
        {
            PaymentDue,
            PaymentConfirmation
        }
        
        
    }
}
