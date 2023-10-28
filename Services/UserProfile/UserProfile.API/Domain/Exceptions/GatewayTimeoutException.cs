namespace UserProfile.API.Domain.Exceptions
{
    public class GatewayTimeoutException : Exception
    {
        #region Variables

        private const string DefaultMessage = "Entity does not exist.";
        public string errorCat { get; }
        public string vendor { get; }
        public string lobSchemaName { get; }
        public int EntityId { get; }
        #endregion

        protected GatewayTimeoutException(string message) : base(message)
        {
        }
        public GatewayTimeoutException(string vendorCode, string lobName) : base(vendorCode)
        {
            errorCat = ErrorCategory.GatewayTimeout.ToString();
            vendor = vendorCode;
            lobSchemaName = lobName;
        }
    }
}
