using VendorConnect.API.Domain.Exceptions;

namespace VendorConnect.API.Domain.Exceptions
{
    public class ConflictException : Exception
    {
        #region Variables

        private const string DefaultMessage = "Entity does not exist.";
        public string errorCat { get; }
        public string vendor { get; }
        public string lobSchemaName { get; }
        public int EntityId { get; }
        #endregion

        protected ConflictException(string message) : base(message)
        {
        }
        public ConflictException(string vendorCode, string lobName) : base(vendorCode)
        {
            errorCat = ErrorCategory.Conflict.ToString();
            vendor = vendorCode;
            lobSchemaName = lobName;
        }
    }
}
