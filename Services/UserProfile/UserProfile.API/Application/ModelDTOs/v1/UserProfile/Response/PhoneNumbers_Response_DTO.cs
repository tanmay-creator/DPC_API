namespace UserProfile.API.Application.ModelDTOs.v1.UserProfile.Response

{
    public record PhoneNumbers_Response_DTO
    {
        //public PhoneNumber [] phoneNumber { get; set; }
        //[Required]
        public string kind { get; init; }

        //[Required]
        public string number { get; init; }

        bool allowSMS { get; init; } = false;
    }
}
