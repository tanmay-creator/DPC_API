namespace UserProfile.API.Domain.Exceptions
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
        ValidationError
    }
}
