namespace UserProfile.API.Domain.Entities
{
    public class Address
    {
        public string[] lines { get; set; } 
        public string city { get; set; } 
        public string regionCode { get; set; }
        public string postalCode { get; set; }
        public string countryCode { get; set; }   
    }
}
