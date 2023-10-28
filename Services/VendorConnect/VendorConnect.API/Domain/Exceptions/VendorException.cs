using System.Reflection.Metadata;

namespace VendorConnect.API.Domain.Exceptions
{
    public class VendorException : Exception
    {
        #region Variables

        public HttpStatusCode StatusCode { get; set; }        
        public string errorCat { get; }
        public string VendorCode { get; }
        public string LobCode { get; }
       
        #endregion

        protected VendorException(string message) : base(message)
        {
        }
        public VendorException(string vendorCode, string lobCode, HttpStatusCode statusCode = HttpStatusCode.OK) : base(vendorCode)
        {
            VendorCode = vendorCode;
            LobCode = lobCode;
            StatusCode = statusCode;
        }
    }
}
