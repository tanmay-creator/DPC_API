namespace UserProfile.API.Application.ModelDTOs.v1.UserProfile.Response

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
