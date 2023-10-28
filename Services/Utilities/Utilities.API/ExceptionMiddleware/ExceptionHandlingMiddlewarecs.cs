using System.Text.Json;
using Utilities.API.ExceptionMiddleware.Exceptions;
using KeyNotFoundException = Utilities.API.ExceptionMiddleware.Exceptions.KeyNotFoundException;
using NotImplementedException = Utilities.API.ExceptionMiddleware.Exceptions.NotImplementedException;
using UnauthorizedAccessException = Utilities.API.ExceptionMiddleware.Exceptions.UnauthorizedAccessException;
//namespace Utilities.API.Controllers;

namespace Utilities.API.ExceptionMiddleware
{
    internal sealed class ExceptionHandlingMiddleware 
    {
        //private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        //public ExceptionHandlingMiddleware()
        //{          
        //    //_logger = logger;
        //}

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }


        //public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) => _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                //_logger.LogError(e, e.Message);

                await HandleExceptionAsync(context, e);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            httpContext.Response.ContentType = "application/json";

            var stackTrace = string.Empty;
            string dpcErrorCode = string.Empty;

            //HttpStatusCode dpc_status = HttpStatusCode.Ambiguous;
            int dpc_status_code = 0;
            string dpc_kind = string.Empty;
            string dpc_error_code = string.Empty;
            string dpc_error_message = string.Empty;

            /*----------- Read the eror codes and messages from Database/Redis or rules engine-------------------*/
            /* -------------------- dpc_kind, dpc_status, dpc_error_code, dpc_error_message   */
            
            var exceptionType = exception.GetType();

            switch (exception)
            {
                case NotFoundException:
                    dpc_status_code = httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    dpc_kind = "Invalid Request";
                    dpc_error_code = "DPC.6000";
                    dpc_error_message = exception.Message;
                    stackTrace = exception.StackTrace;
                    break;                                
                case BadRequestException:
                    dpc_status_code = httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    dpc_kind = "Invalid Request";
                    dpc_error_code = "DPC.6000";
                    dpc_error_message = exception.Message;
                    stackTrace = exception.StackTrace;
                    break;
                case NotImplementedException:
                    dpc_status_code = httpContext.Response.StatusCode = StatusCodes.Status501NotImplemented;
                    dpc_kind = "Invalid Request";
                    dpc_error_code = "DPC.6000";
                    dpc_error_message = exception.Message;
                    stackTrace = exception.StackTrace;
                    break;
                case UnauthorizedAccessException:
                    dpc_status_code = httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    dpc_kind = "Invalid Request";
                    dpc_error_code = "DPC.6000";
                    dpc_error_message = exception.Message;
                    stackTrace = exception.StackTrace;
                    break;
                case KeyNotFoundException:
                    dpc_status_code = httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    dpc_kind = "Invalid Request";
                    dpc_error_code = "DPC.6000";
                    dpc_error_message = exception.Message;
                    stackTrace = exception.StackTrace;
                    break;
                default:
                    dpc_status_code = httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    dpc_kind = "Invalid Request";
                    dpc_error_code = "DPC.6000";
                    dpc_error_message = exception.Message;
                    stackTrace = exception.StackTrace;
                    break;
            }

            var response = new
            {
                error = new
                {
                    status = dpc_status_code,
                    kind = dpc_kind,
                    message = new
                    {
                        code = dpc_error_code,
                        error_message = dpc_error_message
                    }
                }
                //error = exception.Message                
            };

            await httpContext.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }

        //public Task InvokeAsync(HttpContext context, RequestDelegate next)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
