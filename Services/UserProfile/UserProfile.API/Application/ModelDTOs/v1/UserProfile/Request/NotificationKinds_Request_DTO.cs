namespace UserProfile.API.Application.ModelDTOs.v1.UserProfile.Request
{
    public record NotificationKinds_Request_DTO
    {
        enum NotificationKind
        {
            PaymentDue,
            PaymentConfirmation
        }


    }
}
