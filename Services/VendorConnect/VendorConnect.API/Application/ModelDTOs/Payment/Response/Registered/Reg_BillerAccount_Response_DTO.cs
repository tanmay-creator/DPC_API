namespace VendorConnect.API.Application.ModelDTOs.Payment.Response.Registered
{
    public record Reg_BillerAccount_Response_DTO
    {
        public string billerId { get; init; }
        public string billerAccountId { get; init; }
    }
}
