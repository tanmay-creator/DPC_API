using Transactions.API.Domain.Exceptions;

namespace Transaction.API.Domain.Exceptions
{
    public class MethodNotAllowedException : Exception
    {
        #region Variables

        private const string DefaultMessage = "Entity does not exist.";
        public string errorCat { get; }
        public string vendor { get; }
        public string lobSchemaName { get; }
        public int EntityId { get; }
        #endregion

        protected MethodNotAllowedException(string message) : base(message)
        {
        }
        public MethodNotAllowedException(string vendorCode, string lobName) : base(vendorCode)
        {
            errorCat = ErrorCategory.MethodNotAllowed.ToString();
            vendor = vendorCode;
            lobSchemaName = lobName;
        }
    }
}
