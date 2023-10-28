namespace VendorConnect.API.Domain.Exceptions
{
    public class TooManyRequestException : Exception
    {
        #region Variables

        private const string DefaultMessage = "Entity does not exist.";
        public string errorCat { get; }
        public string vendor { get; }
        public string lobSchemaName { get; }
        public int EntityId { get; }
        #endregion

        protected TooManyRequestException(string message) : base(message)
        {
        }
        public TooManyRequestException(string vendorCode, string lobName) : base(vendorCode)
        {
            errorCat = ErrorCategory.TooManyRequest.ToString();
            vendor = vendorCode;
            lobSchemaName = lobName;
        }
    }
}
