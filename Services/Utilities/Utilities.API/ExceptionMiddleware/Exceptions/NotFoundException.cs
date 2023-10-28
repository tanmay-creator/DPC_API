using System;

namespace Utilities.API.ExceptionMiddleware.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string dpcErrorMessage)
            : base(dpcErrorMessage)
        {
        }
    }
}
