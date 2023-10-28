namespace VendorConnect.API.Application.ModelDTOs.UserProfile.Request
{
    public record PhoneNumber_Request_DTO
    {
        //[Required]
        //public string[] kind { get; set; } 
        public string kind { get; init; }

        //[Required]
        public string number { get; init; }

        Boolean allowSMS { get; init; } = false;

    }
}
