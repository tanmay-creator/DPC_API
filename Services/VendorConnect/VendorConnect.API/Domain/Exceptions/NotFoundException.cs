namespace VendorConnect.API.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        #region Variables

        private const string DefaultMessage = "Entity does not exist.";
        public string errorCat { get; }
        public string vendor { get; }
        public string lobSchemaName { get; }
        public int EntityId { get; }

        #endregion

        protected NotFoundException(string message) : base(message)
        {
        }
        public NotFoundException(string vendorCode, string lobName) : base(vendorCode)
        {
            errorCat = ErrorCategory.NotFound.ToString();
            vendor = vendorCode;
            lobSchemaName = lobName;
        }
    }
}
