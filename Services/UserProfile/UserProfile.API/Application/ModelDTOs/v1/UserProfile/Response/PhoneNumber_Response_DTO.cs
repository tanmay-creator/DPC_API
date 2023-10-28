namespace UserProfile.API.Application.ModelDTOs.v1.UserProfile.Response
{
    public record PhoneNumber_Response_DTO
    {
        //[Required]
        public string[] kind { get; init; }

        //[Required]
        public string number { get; init; }

        bool allowSMS { get; init; } = false;

    }
}
