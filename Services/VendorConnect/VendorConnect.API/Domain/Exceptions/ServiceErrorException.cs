namespace VendorConnect.API.Domain.Exceptions
{
    public class ServiceErrorException : Exception
    {
        #region Variables

        private const string DefaultMessage = "Entity does not exist.";
        public string errorCat { get; }
        public string vendor { get; }
        public string lobSchemaName { get; }
        public int EntityId { get; }
        #endregion

        protected ServiceErrorException(string message) : base(message)
        {
        }
        public ServiceErrorException(string vendorCode, string lobName) : base(vendorCode)
        {
            errorCat = ErrorCategory.ServiceError.ToString();
            vendor = vendorCode;
            lobSchemaName = lobName;
        }
    }
}
