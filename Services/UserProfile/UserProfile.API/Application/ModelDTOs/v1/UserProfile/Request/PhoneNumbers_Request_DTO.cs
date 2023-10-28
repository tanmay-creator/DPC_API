namespace UserProfile.API.Application.ModelDTOs.v1.UserProfile.Request
{
    public record PhoneNumbers_Request_DTO
    {
        public string kind { get; init; }

        //[Required]
        public string number { get; init; }

        bool allowSMS { get; init; } = false;
    }
}
