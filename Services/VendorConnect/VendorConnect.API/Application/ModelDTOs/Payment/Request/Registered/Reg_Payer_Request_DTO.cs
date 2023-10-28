namespace VendorConnect.API.Application.ModelDTOs.Payment.Request.Registered
{
    public record Reg_Payer_Request_DTO
    {
        public string kind { get; init; }
        public string userProfileId { get; init; }
        public string firstName { get; init; }
        public string lastName { get; init; }
        public string middleName { get; init; }

        public string nameSuffix { get; init; }
        public string emailAddress { get; init; }
        public Reg_Address_Request_DTO address { get; init; }

        public Reg_PhoneNumbers_Request_DTO[] phoneNumbers { get; init; }
    }
}
