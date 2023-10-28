namespace Transaction.API.Application.ModelDTOs.v1.Payment.Request.Unregistered
{
    public record Unreg_Address_Request_DTO
    {
        public string city { get; init; }
        public string regionCode { get; init; }
        public string postalCode { get; init; }
        public string countryCode { get; init; }
    }
}
