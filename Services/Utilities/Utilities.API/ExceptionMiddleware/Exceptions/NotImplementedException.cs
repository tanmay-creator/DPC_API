namespace Utilities.API.ExceptionMiddleware.Exceptions
{
    public abstract class NotImplementedException : Exception
    {
        protected NotImplementedException(string message)
            : base(message)
        {

        }
    }
}
