using Transactions.API.Domain.Exceptions;

namespace Transaction.API.Domain.Exceptions
{
    public class UnsupportedMediaTypeException : Exception
    {
        #region Variables

        private const string DefaultMessage = "Entity does not exist.";
        public string errorCat { get; }
        public string vendor { get; }
        public string lobSchemaName { get; }
        public int EntityId { get; }
        #endregion

        protected UnsupportedMediaTypeException(string message) : base(message)
        {
        }
        public UnsupportedMediaTypeException(string vendorCode, string lobName) : base(vendorCode)
        {
            errorCat = ErrorCategory.UnsupportedMediaType.ToString();
            vendor = vendorCode;
            lobSchemaName = lobName;
        }
    }
}
