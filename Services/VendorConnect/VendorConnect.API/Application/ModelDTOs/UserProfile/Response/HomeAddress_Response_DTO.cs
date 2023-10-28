namespace VendorConnect.API.Application.ModelDTOs.UserProfile.Response

{
    public record HomeAddress_Response_DTO
    {
        public string[] lines { get; init; }    
        public string city { get; init; } 
        public string regionCode { get; init; }
        public string postalCode { get; init; }
        public string countryCode { get; init; }   
    }
}
