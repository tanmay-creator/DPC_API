using VendorConnect.API.Domain.Exceptions;

namespace VendorConnect.API.Domain.Exceptions
{
    public class BadRequestException : Exception
    {
        #region Variables

        private const string DefaultMessage = "Entity does not exist.";
        public string errorCat { get; }
        public string vendor { get; }
        public string lobSchemaName { get; }
        public int EntityId { get; }
        #endregion

        protected BadRequestException(string message) : base(message)
        {
        }
        public BadRequestException(string vendorCode, string lobName) : base(vendorCode)
        {
            errorCat = ErrorCategory.BadRequest.ToString();
            vendor = vendorCode;
            lobSchemaName = lobName;
        }
    }
}
