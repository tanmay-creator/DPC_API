namespace VendorConnect.API.Application.ModelDTOs.Payment.Response.Unregistered
{
    public record Unreg_BillerAccount_Response_DTO
    {
        public string billerId { get; init; }
        public string billerAccountId { get; init; }
    }
}
