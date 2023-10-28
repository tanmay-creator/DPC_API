namespace VendorConnect.API.Domain.Exceptions
{
    public class RequestTimeoutException : Exception
    {
        #region Variables

        private const string DefaultMessage = "Entity does not exist.";
        public string errorCat { get; }
        public string vendor { get; }
        public string lobSchemaName { get; }
        public int EntityId { get; }
        #endregion

        protected RequestTimeoutException(string message) : base(message)
        {
        }
        public RequestTimeoutException(string vendorCode, string lobName) : base(vendorCode)
        {
            errorCat = ErrorCategory.RequestTimeout.ToString();
            vendor = vendorCode;
            lobSchemaName = lobName;
        }
    }
}
