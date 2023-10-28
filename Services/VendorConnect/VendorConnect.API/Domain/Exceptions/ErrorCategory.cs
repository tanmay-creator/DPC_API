namespace VendorConnect.API.Domain.Exceptions
{
    public enum ErrorCategory
    {
        BadRequest,
        Conflict,
        GatewayTimeout,
        InvalidRequest,
        MethodNotAllowed,
        NotAcceptable,
        NotFound,
        NoContent,
        OperationDeclined,
        RequestTimeout,
        ResourceAlreadyExist,
        ResourceNotFound,
        ServiceError,
        ServiceNotAvailable,
        TooManyRequest,
        Unauthorized,
        UnsupportedHttpMethod,
        UnsupportedMediaType,
        ValidationError,
        CircuitBreaker
    }
}
