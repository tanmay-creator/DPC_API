namespace UserProfile.API.Application.ModelDTOs.v1.UserProfile.Request
{
    public record PhoneNumber_Request_DTO
    {
        //[Required]
        //public string[] kind { get; set; } 
        public string kind { get; init; }

        //[Required]
        public string number { get; init; }

        bool allowSMS { get; init; } = false;

    }
}
