using VendorConnect.API.Application.Services.Services.Abstraction;
using VendorConnect.API.Domain.Exceptions;
using System.Text.Json;
using VendorConnect.API.Domain.Entities;
using FundingAccount.API.Application.Services;

namespace VendorConnect.API.Infrastructure.ExceptionMiddleware
{
    public class ExceptionHandlingMiddleware //: IMiddleware
    {
        //private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IUtilityService _utilityService;
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


        //public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) => _logger = logger;

        public async Task InvokeAsync(HttpContext context, IUtilityService utilityService)
        {
            try
            {
                await _next(context);
            }
            catch (VendorException vex)
            {
                HandleVendorExceptionAsync(context, vex, utilityService);
            }
            catch (Exception e)
            {
                //_logger.LogError(e, e.Message);

                HandleExceptionAsync(context, e, utilityService);
            }
        }
        private async Task<IList<ErrorDetails>> GetErrDetailsFromUtility(IUtilityService utilityService, string errorCategory, string vendorCode, string lobCode)
        {
            HttpResponseMessage response = await utilityService.GetApiErrorDetails(errorCategory, vendorCode, lobCode);
            var jsonObject = await response.Content.ReadAsStringAsync();
            List<ErrorDetails> lstErrorDetails = JsonConvert.DeserializeObject<List<ErrorDetails>>(jsonObject);

            if(lstErrorDetails.Count == 0)
            {
                response = await utilityService.GetApiErrorDetails("InternalServer", vendorCode, lobCode);
                jsonObject = await response.Content.ReadAsStringAsync();
                lstErrorDetails = JsonConvert.DeserializeObject<List<ErrorDetails>>(jsonObject);
            }

            return lstErrorDetails;
        }
        private void HandleExceptionAsync(HttpContext httpContext, Exception exception, IUtilityService utilityService)
        {
            
            var exceptiontype = exception.GetType();



            httpContext.Response.ContentType = "application/json";

            var stackTrace = string.Empty;
            string dpcErrorCode = string.Empty;

            int dpc_status = 0;
            string dpc_error_code = string.Empty;
            string dpc_error_type = string.Empty;
            string dpc_error_description = string.Empty;

            var exceptionType = exception.GetType();


            if (exceptionType == typeof(BadRequestException))
            {
                var exceptionValues = (BadRequestException)exception;
                //CommonMehotd(erro, lob, ven)
                //lisoferrordetails
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "BadRequest", exceptionValues.vendor, exceptionValues.lobSchemaName).Result;

                //dpc_error_code = listerrordetails
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();


            }
            else if (exceptionType == typeof(NotFoundException))
            {
                var exceptionValues = (NotFoundException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;

                //dpc_error_code = listerrordetails
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(CircuitBreakerException))
            {
                var exceptionValues = (CircuitBreakerException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(NoContentException))
            {
                var exceptionValues = (NoContentException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(ConflictException))
            {
                var exceptionValues = (ConflictException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(GatewayTimeoutException))
            {
                var exceptionValues = (GatewayTimeoutException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(InvalidRequestException))
            {
                var exceptionValues = (InvalidRequestException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(MethodNotAllowedException))
            {
                var exceptionValues = (MethodNotAllowedException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(NotAcceptableException))
            {
                var exceptionValues = (NotAcceptableException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(OperationDeclinedException))
            {
                var exceptionValues = (OperationDeclinedException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(RequestTimeoutException))
            {
                var exceptionValues = (RequestTimeoutException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(ResourceAlreadyExistException))
            {
                var exceptionValues = (ResourceAlreadyExistException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(ResourceNotFoundException))
            {
                var exceptionValues = (ResourceNotFoundException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(ServiceErrorException))
            {
                var exceptionValues = (ServiceErrorException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(ServiceNotAvailableException))
            {
                var exceptionValues = (ServiceNotAvailableException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(TooManyRequestException))
            {
                var exceptionValues = (TooManyRequestException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(UnauthorizedException))
            {
                var exceptionValues = (UnauthorizedException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(UnsupportedHttpMethodException))
            {
                var exceptionValues = (UnsupportedHttpMethodException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(UnsupportedMediaTypeException))
            {
                var exceptionValues = (UnsupportedMediaTypeException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else if (exceptionType == typeof(ValidationErrorException))
            {
                var exceptionValues = (ValidationErrorException)exception;
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, exceptionValues.errorCat, exceptionValues.vendor, exceptionValues.lobSchemaName).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }
            else
            {
                
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "BadRequest", "Aci", "Met_sfd").Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
            }



            DPCErrorModel dPCErrorModel = new DPCErrorModel();
            dPCErrorModel.error = new Domain.Entities.Error
            {
                status = dpc_status,
                code = dpc_error_code,
                type = dpc_error_type,
                description = dpc_error_description,
            };
            _logger.LogError(exception, exception.Message, dPCErrorModel);
           

            //responseMessage.Content = new StringContent(JsonConvert.SerializeObject(response));
            httpContext.Response.StatusCode = dpc_status;
            httpContext.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(dPCErrorModel));
            // return responseMessage;
        }

        private void HandleVendorExceptionAsync(HttpContext httpContext, Exception exception, IUtilityService utilityService)
        {
            #region "Variable Initialization"
            int dpc_status = 0;
            string dpc_error_code = string.Empty;
            string dpc_error_type = string.Empty;
            string dpc_error_description = string.Empty;
            VendorException vendorExcpValues = null;
            #endregion

            var exceptionType = exception.GetType();
            httpContext.Response.ContentType = "application/json";

            if (exceptionType == typeof(VendorException))
            {
                vendorExcpValues = (VendorException)exception;
            }

            if (vendorExcpValues.StatusCode == HttpStatusCode.BadRequest)
            {
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "BadRequest", vendorExcpValues.VendorCode, vendorExcpValues.LobCode).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            else if (vendorExcpValues.StatusCode == HttpStatusCode.NotFound)
            {
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "NotFound", vendorExcpValues.VendorCode, vendorExcpValues.LobCode).Result;

                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            }
            else if (vendorExcpValues.StatusCode == HttpStatusCode.Conflict)
            {
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "Conflict", vendorExcpValues.VendorCode, vendorExcpValues.LobCode).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            }
            else if (vendorExcpValues.StatusCode == HttpStatusCode.GatewayTimeout)
            {
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "GatewayTimeout", vendorExcpValues.VendorCode, vendorExcpValues.LobCode).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
                httpContext.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
            }
            else if (vendorExcpValues.StatusCode == HttpStatusCode.MethodNotAllowed)
            {
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "MethodNotAllowed", vendorExcpValues.VendorCode, vendorExcpValues.LobCode).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
                httpContext.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
            }
            else if (vendorExcpValues.StatusCode == HttpStatusCode.NotAcceptable)
            {

                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "NotAcceptable", vendorExcpValues.VendorCode, vendorExcpValues.LobCode).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
                httpContext.Response.StatusCode = StatusCodes.Status406NotAcceptable;
            }
            else if (vendorExcpValues.StatusCode == HttpStatusCode.NoContent)
            {

                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "NoContent", vendorExcpValues.VendorCode, vendorExcpValues.LobCode).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
                httpContext.Response.StatusCode = StatusCodes.Status204NoContent;
            }
            else if (vendorExcpValues.StatusCode == HttpStatusCode.Forbidden)
            {
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "Forbidden", vendorExcpValues.VendorCode, vendorExcpValues.LobCode).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
                httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            }
            else if (vendorExcpValues.StatusCode == HttpStatusCode.RequestTimeout)
            {
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "RequestTimeout", vendorExcpValues.VendorCode, vendorExcpValues.LobCode).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim().Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
                httpContext.Response.StatusCode = StatusCodes.Status408RequestTimeout;
            }
            else if (vendorExcpValues.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "ServiceUnavailable", vendorExcpValues.VendorCode, vendorExcpValues.LobCode).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
                httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            }
            else if (vendorExcpValues.StatusCode == HttpStatusCode.TooManyRequests)
            {
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "TooManyRequests", vendorExcpValues.VendorCode, vendorExcpValues.LobCode).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
                httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            }
            else if (vendorExcpValues.StatusCode == HttpStatusCode.Unauthorized)
            {
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "ServiceUnavailable", vendorExcpValues.VendorCode, vendorExcpValues.LobCode).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else if (vendorExcpValues.StatusCode == HttpStatusCode.UnsupportedMediaType)
            {
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "TooManyRequests", vendorExcpValues.VendorCode, vendorExcpValues.LobCode).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
                httpContext.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
            }
            else if (vendorExcpValues.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "UnprocessableEntity", vendorExcpValues.VendorCode, vendorExcpValues.LobCode).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
                httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            }
            else
            {

                IList<ErrorDetails> lstErrorDetails = GetErrDetailsFromUtility(utilityService, "InternalServerError", vendorExcpValues.VendorCode, vendorExcpValues.LobCode).Result;
                dpc_error_code = lstErrorDetails.
                                         Select(c => c.DPCErrorCode.Trim()).FirstOrDefault().ToString();
                dpc_error_type = lstErrorDetails.
                                 Select(c => c.ErrorCategory.Trim()).FirstOrDefault().ToString();
                dpc_error_description = lstErrorDetails.
                                 Select(c => c.DPCErrorDescription).FirstOrDefault().ToString();
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }


            DPCErrorModel dPCErrorModel = new DPCErrorModel();
            dPCErrorModel.error = new Domain.Entities.Error
            {
                status = dpc_status,
                code = dpc_error_code,
                type = dpc_error_type,
                description = dpc_error_description,
            };

            _logger.LogError(exception, exception.Message, dPCErrorModel);


            //responseMessage.Content = new StringContent(JsonConvert.SerializeObject(response));
           // httpContext.Response.StatusCode = dpc_status;
            httpContext.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(dPCErrorModel));
            // return responseMessage;
        }

    }
}
