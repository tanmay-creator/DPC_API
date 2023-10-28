namespace VendorConnect.API.Domain.Exceptions
{
    public class CircuitBreakerException : Exception
    {
        #region Variables

        private const string DefaultMessage = "Entity does not exist.";
        public string errorCat { get; }
        public string vendor { get; }
        public string lobSchemaName { get; }
        public int EntityId { get; }
        #endregion

        protected CircuitBreakerException(string message) : base(message)
        {
        }
        public CircuitBreakerException(string vendorCode, string lobName) : base(vendorCode)
        {
            errorCat = ErrorCategory.CircuitBreaker.ToString();
            vendor = vendorCode;
            lobSchemaName = lobName;
        }
    }
}
