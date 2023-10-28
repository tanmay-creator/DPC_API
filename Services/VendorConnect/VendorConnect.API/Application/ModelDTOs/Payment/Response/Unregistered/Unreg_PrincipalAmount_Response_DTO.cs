namespace VendorConnect.API.Application.ModelDTOs.Payment.Response.Unregistered
{
    public record Unreg_PrincipalAmount_Response_DTO
    {
        public long value { get; set; }

        public int precision { get; set; }
        public string currencyCode { get; set; }
    }
}
