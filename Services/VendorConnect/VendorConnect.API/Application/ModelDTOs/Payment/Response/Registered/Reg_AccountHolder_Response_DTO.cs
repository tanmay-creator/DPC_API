namespace VendorConnect.API.Application.ModelDTOs.Payment.Response.Registered
{
    public record Reg_AccountHolder_Response_DTO
    {
        public string name { get; init; }
        public Reg_FundingAddress_Response_DTO address { get; init; }
    }
}
