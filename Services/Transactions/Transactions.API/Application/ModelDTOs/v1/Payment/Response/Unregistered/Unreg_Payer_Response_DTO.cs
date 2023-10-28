namespace Transaction.API.Application.ModelDTOs.v1.Payment.Response.Unregistered
{
    public record Unreg_Payer_Response_DTO
    {
        public string kind { get; init; }
        public Unreg_Address_Response_DTO address { get; init; }
        public string emailAddress { get; init; }
        public Unreg_PhoneNumbers_Response_DTO[] phoneNumbers { get; init; }
        public string firstName { get; init; }
        public string middleName { get; init; }
        public string lastName { get; init; }
        public string nameSuffix { get; init; }
    }
}
