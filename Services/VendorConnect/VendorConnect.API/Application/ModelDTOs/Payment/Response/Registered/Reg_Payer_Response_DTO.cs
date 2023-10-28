namespace VendorConnect.API.Application.ModelDTOs.Payment.Response.Registered
{
    public record Reg_Payer_Response_DTO
    {
        public string kind { get; init; }
        public Reg_Address_Response_DTO address { get; init; }
        public string emailAddress { get; init; }
        public Reg_PhoneNumbers_Response_DTO[] phoneNumbers { get; init; }
        public string firstName { get; init; }
        public string middleName { get; init; }
        public string lastName { get; init; }
        public string nameSuffix { get; init; }
        public string userProfileId { get; init;}
    }
}
