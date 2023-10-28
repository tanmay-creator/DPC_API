namespace VendorConnect.API.Application.ModelDTOs.Payment.Response.Unregistered
{
    public record Unreg_Message_Response_DTO
    {
        public string code { get; init; }
        public string @default { get; init; }
    }
}
