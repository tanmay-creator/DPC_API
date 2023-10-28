using Transactions.API.Domain.Exceptions;

namespace Transaction.API.Domain.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        #region Variables

        private const string DefaultMessage = "Entity does not exist.";
        public string errorCat { get; }
        public string vendor { get; }
        public string lobSchemaName { get; }
        public int EntityId { get; }
        #endregion

        protected ResourceNotFoundException(string message) : base(message)
        {
        }
        public ResourceNotFoundException(string vendorCode, string lobName) : base(vendorCode)
        {
            errorCat = ErrorCategory.ResourceNotFound.ToString();
            vendor = vendorCode;
            lobSchemaName = lobName;
        }
    }
}
