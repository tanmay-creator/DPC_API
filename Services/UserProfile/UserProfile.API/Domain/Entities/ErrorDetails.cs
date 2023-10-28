namespace UserProfile.API.Domain.Entities
{
    public class ErrorDetails
    {
        public string ErrorSource { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorCategory { get; set; }
        public string ErrorDescription { get; set; }
        public string DPCErrorCode { get; set; }
        public string DPCErrorDescription { get; set; }
        public string VendorID { get; set; }
        public string VendorCode { get; set; }
        public string ErrorActive { get; set; }
    }
}
