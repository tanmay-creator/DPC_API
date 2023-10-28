using Transactions.API.Domain.Exceptions;

namespace Transaction.API.Domain.Exceptions
{
    public class ResourceAlreadyExistException : Exception
    {
        #region Variables

        private const string DefaultMessage = "Entity does not exist.";
        public string errorCat { get; }
        public string vendor { get; }
        public string lobSchemaName { get; }
        public int EntityId { get; }
        #endregion

        protected ResourceAlreadyExistException(string message) : base(message)
        {
        }
        public ResourceAlreadyExistException(string vendorCode, string lobName) : base(vendorCode)
        {
            errorCat = ErrorCategory.ResourceAlreadyExist.ToString();
            vendor = vendorCode;
            lobSchemaName = lobName;
        }
    }
}
