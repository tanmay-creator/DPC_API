namespace VendorConnect.API.Application.ModelDTOs
{
    public record AuthResponse
    {
        public string access_token { get; init; }
        public string token_type { get; init; }
        public int expires_in { get; init; }
    }
}
