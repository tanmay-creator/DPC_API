namespace VendorConnect.API.Application.ModelDTOs.Payment.Response.Unregistered
{
    public record Unreg_AccountHolder_Response_DTO
    {
        public string name { get; init; }
        public Unreg_FundingAddress_Response_DTO address { get; init; }
    }
}
