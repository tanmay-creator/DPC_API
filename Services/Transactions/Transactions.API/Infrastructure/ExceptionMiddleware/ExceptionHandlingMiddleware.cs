using Newtonsoft.Json;
using Transaction.API.Application.Services.v1.Services.Abstraction;
using Transaction.API.Domain.Entities;
using Transaction.API.Domain.Exceptions;
using Transactions.API.Domain.Exceptions;
using Error = Transaction.API.Domain.Entities.Error;

namespace Transaction.API.Infrastructure.ExceptionMiddleware
{
    public class ExceptionHandlingMiddleware //: IMiddleware
    {
        //private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IUtilitiesService _utilityService;
        //public ExceptionHandlingMiddleware()
        //{          
        //    //_logger = logger;
        //}

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            //_utilityService = utilityService;
        }



        public async Task InvokeAsync(HttpContext context, IUtilitiesService utilityService)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {

                HandleExceptionAsync(context, e, utilityService);
            }
        }
        private IList<ErrorDetails> GetErrDetailsFromUtility(IUtilitiesService utilityService, string errorCategory, string vendorCode, string lobCode)
        {
            HttpResponseMessage response = utilityService.GetApiErrorDetails(errorCategory, vendorCode, lobCode);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }
            //var errorDetaildContent =  response.Content.ReadAsStream();
            using var errorDetaildContent = new StreamReader(response.Content.ReadAsStream());

            var errList = errorDetaildContent.ReadToEnd();

            List<ErrorDetails> lstErrorDetails = JsonConvert.DeserializeObject<List<ErrorDetails>>(errList);
            if (lstErrorDetails.Count == 0)
            {

                response = utilityService.GetApiErrorDetails("InternalServer", vendorCode, lobCode);
                using var errorDetailContent2 = new StreamReader(response.Content.ReadAsStream());
                var errlist2 = errorDetailContent2.ReadToEnd();
                lstErrorDetails = JsonConvert.DeserializeObject<List<ErrorDetails>>(errlist2);
            }

            return lstErrorDetails;
        }
        private void HandleExceptionAsync(HttpContext httpContext, Exception exception, IUtilitiesService utilityService)
        {

            var exceptiontype = exception.GetType();



            //var errorResponseContent = await errorResponse.Content.ReadAsStringAsync();
            httpContext.Response.ContentType = "application/json";

            var stackTrace = string.Empty;
            string dpcErrorCode = string.Empty;

            //HttpStatusCode dpc_status = HttpStatusCode.Ambiguous;
            int dpc_status = 0;
            string dpc_error_code = string.Empty;
            string dpc_error_type = string.Empty;
            string dpc_error_description = string.Empty;

            var exceptionType = exception.GetType();


            if (exceptionType == typeof(BadRequestException))
            {
                var exceptionValues = (BadRequestException)exception;

                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName);

                dpc_status = (int)HttpStatusCode.BadRequest;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();


            }
            else if (exceptionType == typeof(NotFoundException))
            {
                var exceptionValues = (NotFoundException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName);
                dpc_status = (int)HttpStatusCode.NotFound;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(NoContentException))
            {
                var exceptionValues = (NoContentException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName);
                dpc_status = (int)HttpStatusCode.NoContent;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }

            else if (exceptionType == typeof(ConflictException))
            {
                var exceptionValues = (ConflictException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName);
                dpc_status = (int)HttpStatusCode.Conflict;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(GatewayTimeoutException))
            {
                var exceptionValues = (GatewayTimeoutException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName);
                dpc_status = (int)HttpStatusCode.GatewayTimeout;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(InvalidRequestException))
            {
                var exceptionValues = (InvalidRequestException)exception;

                dpc_status = (int)HttpStatusCode.BadRequest;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName);
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(MethodNotAllowedException))
            {
                var exceptionValues = (MethodNotAllowedException)exception;

                dpc_status = (int)HttpStatusCode.MethodNotAllowed;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName);
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(NotAcceptableException))
            {
                var exceptionValues = (NotAcceptableException)exception;

                dpc_status = (int)HttpStatusCode.NotAcceptable;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName);
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(OperationDeclinedException))
            {
                var exceptionValues = (OperationDeclinedException)exception;


                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName);

                dpc_status = (int)HttpStatusCode.Forbidden;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(RequestTimeoutException))
            {
                var exceptionValues = (RequestTimeoutException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName);

                dpc_status = (int)HttpStatusCode.RequestTimeout;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(ResourceAlreadyExistException))
            {
                var exceptionValues = (ResourceAlreadyExistException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName);

                dpc_status = (int)HttpStatusCode.Conflict;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(ResourceNotFoundException))
            {
                var exceptionValues = (ResourceNotFoundException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "", exceptionValues.vendor, exceptionValues.lobSchemaName);

                dpc_status = (int)HttpStatusCode.NotFound;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(ServiceErrorException))
            {
                var exceptionValues = (ServiceErrorException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName);

                dpc_status = (int)HttpStatusCode.InternalServerError;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(ServiceNotAvailableException))
            {
                var exceptionValues = (ServiceNotAvailableException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName);

                dpc_status = (int)HttpStatusCode.ServiceUnavailable;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(TooManyRequestException))
            {
                var exceptionValues = (TooManyRequestException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName);

                dpc_status = (int)HttpStatusCode.TooManyRequests;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(UnauthorizedException))
            {
                var exceptionValues = (UnauthorizedException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName);

                dpc_status = (int)HttpStatusCode.Unauthorized;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(UnsupportedHttpMethodException))
            {
                var exceptionValues = (UnsupportedHttpMethodException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName);

                dpc_status = (int)HttpStatusCode.MethodNotAllowed;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(UnsupportedMediaTypeException))
            {
                var exceptionValues = (UnsupportedMediaTypeException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName);

                dpc_status = (int)HttpStatusCode.UnsupportedMediaType;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(ValidationErrorException))
            {
                var exceptionValues = (ValidationErrorException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName);

                dpc_status = (int)HttpStatusCode.UnprocessableEntity;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else
            {

                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "InternalServer", "Aci", "Mer_sfd");

                dpc_status = (int)HttpStatusCode.InternalServerError;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }

            DPCErrorModel dPCErrorModel = new DPCErrorModel();
            dPCErrorModel.error = new Error
            {
                status = dpc_status,
                code = dpc_error_code,
                type = dpc_error_type,
                description = dpc_error_description,
            };

            //var response = new
            //{
            //    error = new
            //    {
            //        status = dpc_status,
            //        code = dpc_error_code,
            //        type = dpc_error_type,
            //        description = dpc_error_description
            //    }
            //    //error = exception.Message                
            //};

            //_logger.LogError(exception, exception.Message, response);

            httpContext.Response.StatusCode = dpc_status;
            httpContext.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(dPCErrorModel));

        }
    }
}
