using System;

namespace Utilities.API.ExceptionMiddleware.Exceptions
{
    public abstract class KeyNotFoundException : Exception
    {
        protected KeyNotFoundException(string message)
            : base(message)
        {
            
        }
    }
}