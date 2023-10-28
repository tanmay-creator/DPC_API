namespace Utilities.API.ExceptionMiddleware.Exceptions
{
    public abstract class UnauthorizedAccessException : Exception
    {
        protected UnauthorizedAccessException(string message)
            : base(message)
        {

        }
    }
}
