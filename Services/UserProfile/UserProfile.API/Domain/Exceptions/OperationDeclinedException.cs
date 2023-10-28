namespace UserProfile.API.Domain.Exceptions
{
    public class OperationDeclinedException : Exception
    {
        #region Variables

        private const string DefaultMessage = "Entity does not exist.";
        public string errorCat { get; }
        public string vendor { get; }
        public string lobSchemaName { get; }
        public int EntityId { get; }
        #endregion

        protected OperationDeclinedException(string message) : base(message)
        {
        }
        public OperationDeclinedException(string vendorCode, string lobName) : base(vendorCode)
        {
            errorCat = ErrorCategory.OperationDeclined.ToString();
            vendor = vendorCode;
            lobSchemaName = lobName;
        }
    }
}
