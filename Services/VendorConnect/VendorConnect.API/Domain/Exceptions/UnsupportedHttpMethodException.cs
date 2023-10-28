namespace VendorConnect.API.Domain.Exceptions
{
    public class UnsupportedHttpMethodException : Exception
    {

        #region Variables

        private const string DefaultMessage = "Entity does not exist.";
        public string errorCat { get; }
        public string vendor { get; }
        public string lobSchemaName { get; }
        public int EntityId { get; }
        #endregion

        protected UnsupportedHttpMethodException(string message) : base(message)
        {
        }
        public UnsupportedHttpMethodException(string vendorCode, string lobName) : base(vendorCode)
        {
            errorCat = ErrorCategory.UnsupportedHttpMethod.ToString();
            vendor = vendorCode;
            lobSchemaName = lobName;
        }
    }
}
