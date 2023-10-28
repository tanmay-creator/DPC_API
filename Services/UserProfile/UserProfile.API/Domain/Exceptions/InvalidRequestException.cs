namespace UserProfile.API.Domain.Exceptions
{
    public class InvalidRequestException : Exception
    {
        #region Variables

        private const string DefaultMessage = "Entity does not exist.";
        public string errorCat { get; }
        public string vendor { get; }
        public string lobSchemaName { get; }
        public int EntityId { get; }
        #endregion

        protected InvalidRequestException(string message) : base(message)
        {
        }
        public InvalidRequestException(string vendorCode, string lobName) : base(vendorCode)
        {
            errorCat = ErrorCategory.InvalidRequest.ToString();
            vendor = vendorCode;
            lobSchemaName = lobName;
        }
    }
}
