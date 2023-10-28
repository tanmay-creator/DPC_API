namespace UserProfile.API.Application.ModelDTOs.v1.UserProfile.Response
{
    public record NotificationKinds_Response_DTO
    {
        enum NotificationKind
        {
            PaymentDue,
            PaymentConfirmation
        }


    }
}
