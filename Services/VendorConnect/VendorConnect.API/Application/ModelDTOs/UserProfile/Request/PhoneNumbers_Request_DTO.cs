namespace VendorConnect.API.Application.ModelDTOs.UserProfile.Request
{
    public record PhoneNumbers_Request_DTO
    {
        //public PhoneNumber[] phoneNumber { get; set; } 

        public string kind { get; init; }

        //[Required]
        public string number { get; init; }

        bool allowSMS { get; init; } = false;
    }
}
