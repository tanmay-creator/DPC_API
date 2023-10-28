namespace VendorConnect.API.Application.ModelDTOs.Payment.Request.Unregistered
{
    public record Unreg_Payer_Request_DTO
    {
        public string kind { get; init; }
        public string firstName { get; init; }
        public string lastName { get; init; }
        public string middleName { get; init; }

        public string nameSuffix { get; init; }
        public string emailAddress { get; init; }
        public Unreg_Address_Request_DTO address { get; init; }

        public Unreg_PhoneNumbers_Request_DTO[] phoneNumbers { get; init; }
    }
}
