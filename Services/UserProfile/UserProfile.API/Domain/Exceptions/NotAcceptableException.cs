namespace UserProfile.API.Domain.Exceptions
{
    public class NotAcceptableException : Exception
    {
        #region Variables

        private const string DefaultMessage = "Entity does not exist.";
        public string errorCat { get; }
        public string vendor { get; }
        public string lobSchemaName { get; }
        public int EntityId { get; }
        #endregion

        protected NotAcceptableException(string message) : base(message)
        {
        }
        public NotAcceptableException( string vendorCode, string lobName) : base(vendorCode)
        {
            errorCat = ErrorCategory.NotAcceptable.ToString();
            vendor = vendorCode;
            lobSchemaName = lobName;
        }
    }
}
