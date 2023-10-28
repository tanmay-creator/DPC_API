using System;

namespace Utilities.API.ExceptionMiddleware.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message)
            : base(message)
        {
            
        }
    }
}