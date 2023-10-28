using Transactions.API.Domain.Exceptions;

namespace Transaction.API.Domain.Exceptions
{
    public class ValidationErrorException : Exception
    {

        #region Variables

        private const string DefaultMessage = "Entity does not exist.";
        public string errorCat { get; }
        public string vendor { get; }
        public string lobSchemaName { get; }
        public int EntityId { get; }
        #endregion

        protected ValidationErrorException(string message) : base(message)
        {
        }
        public ValidationErrorException(string vendorCode, string lobName) : base(vendorCode)
        {
            errorCat = ErrorCategory.ValidationError.ToString();
            vendor = vendorCode;
            lobSchemaName = lobName;
        }
    }
}
