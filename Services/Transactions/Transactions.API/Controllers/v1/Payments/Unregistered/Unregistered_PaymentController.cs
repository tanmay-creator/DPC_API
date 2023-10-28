using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Transaction.API.Application.ModelDTOs.v1.Payment.Request.Unregistered;
using Transaction.API.Application.ModelDTOs.v1.Payment.Response.Unregistered;
using Transaction.API.Application.Services.v1.Services.Abstraction;
using Transaction.API.Domain.Entities;
using Transaction.API.Domain.Exceptions;
using Transactions.API.Domain.Exceptions;

namespace Transaction.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    public class Unregistered_PaymentController : ControllerBase
    {
        #region Payment Controller Variables
        private readonly IUtilitiesService _utilityService;
        private readonly IUnregisteredPaymentService _paymentService;
        private readonly ILogger<Registered_PaymentController> _logger;
        private readonly string _schemaName;
        private string _validationResponseContent;
        private AppSettings _appSettings { get; }

        #region Unregistered
        private readonly string _unregPaymentSchemaName = "Payment/Payment_Request_Schema_Unregistered.json";
        private readonly string _paymentCardResponseUnregSchemaName = "Payment/Payment_Card_Response_Schema_Unregistered.json";
        private readonly string _paymentAchResponseUnregSchemaName = "Payment/Payment_Ach_Response_Schema_Unregistered.json";
        private HttpResponseMessage _unregPaymentReqValidation;
        private HttpResponseMessage _unregPaymentResValidation;
        private HttpResponseMessage _unregPaymentResponse;
        private string _unregPaymentResponseContent;
        private string _vendorCode;
        private string _lobCode;
        #endregion

        #region Registered
        private readonly string _regPaymentSchemaName = "Payment_Request_Schema_Registered.json";
        private readonly string _paymentCardResponseRegSchemaName = "Payment_Card_Response_Schema_Registered.json";
        private readonly string _paymentAchResponseRegSchemaName = "Payment_Ach_Response_Schema_Registered.json";
        private HttpResponseMessage _regPaymentReqValidation;
        private HttpResponseMessage _regPaymentResValidation;
        private HttpResponseMessage _regPaymentResponse;
        private string _regPaymentResponseContent;
        #endregion
        #endregion

        public Unregistered_PaymentController(IUtilitiesService utilityService, IUnregisteredPaymentService paymentService, ILogger<Registered_PaymentController> logger, IOptionsSnapshot<AppSettings> appSettings)
        {
            _utilityService = utilityService;
            _paymentService = paymentService;
            _logger = logger;
            _appSettings = appSettings.Value;
            _schemaName = _appSettings.SchemaName;

        }
        #region Payment Unregistered User

        /// <summary>
        /// Get the payment request, validate the request if it is validated then pass this request to Payment Service class.
        /// Get the Response from Payment Service class, validate the response if it is validated return the response.
        /// In case of errors and unsuccessful validation return proper message.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/v{version:apiVersion}/unregistered/payments")]
        //[Route("api/[controller]/unregistered/payment")]
        public async Task<IActionResult> PaymentUnregisteredUser()
        {
            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];

            try
            {
                _logger.LogInformation("\nPayment Microservice called for UNREGISTERED USER.\n");
                // Get header values
               
                string paymentJsonBody = string.Empty;
                var reader = new StreamReader(Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false);
                paymentJsonBody = await reader.ReadToEndAsync();

                if (paymentJsonBody == null)

                {
                    throw new BadRequestException(_vendorCode, _lobCode);
                }

                JObject data = JObject.Parse(paymentJsonBody);

                _unregPaymentReqValidation = await _utilityService.ValidateInboundRequest(paymentJsonBody, _unregPaymentSchemaName, vendorCode, lobCode);


                if (_unregPaymentReqValidation.StatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation("\nPayment Controller Unregistered: Request Validation Successfull for Unregistered Payment, now will start processing payment\n");
                    //_validationResponseContent = await _unregPaymentReqValidation.Content.ReadAsStringAsync();

                    Unreg_Payment_Request_DTO requestDto = JsonConvert.DeserializeObject<Unreg_Payment_Request_DTO>(paymentJsonBody);

                    _unregPaymentResponse = await _paymentService.MakeUnregisteredPayment(requestDto, vendorCode, lobCode);
                    //JObject aciPaymentResponseJsonBody = JObject.Parse(_unregPaymentResponseContent);
                    if (_unregPaymentResponse.StatusCode == HttpStatusCode.OK)
                    {
                        _logger.LogInformation("\nPayment Controller Unregistered: Response Fetched Successfull. Now will proceed for the response validation.\n");
                        _unregPaymentResponseContent = await _unregPaymentResponse.Content.ReadAsStringAsync();
                        if (_unregPaymentResponseContent == null)
                        {
                            throw new NoContentException(vendorCode,lobCode);
                        }
                        JObject unregResponseBody = JObject.Parse(_unregPaymentResponseContent);
                        

                        bool kindExist = unregResponseBody.SelectTokens("fundingAccountSummary.kind").Any();
                        if (!kindExist)
                        {
                            throw new NotAcceptableException(vendorCode, lobCode);
                        }
                        string responseKind = unregResponseBody.SelectToken("fundingAccountSummary.kind").ToString();
                        responseKind = responseKind.ToLower();

                        if (responseKind.Equals("ach"))
                        {


                            _unregPaymentResValidation = await _utilityService.ValidateInboundRequest(_unregPaymentResponseContent, _paymentAchResponseUnregSchemaName, vendorCode, lobCode);
                            _validationResponseContent = await _unregPaymentResValidation.Content.ReadAsStringAsync();
                            if (_unregPaymentResValidation.StatusCode == HttpStatusCode.OK)
                            {
                                _logger.LogInformation("\nPayment Controller Unregistered: Ach Response Validation Successfull for Unregistered Payment\n");
                                Unreg_Ach_Payment_Response_DTO responseAchDto = JsonConvert.DeserializeObject<Unreg_Ach_Payment_Response_DTO>(_unregPaymentResponseContent);
                                return new ObjectResult(responseAchDto);
                            }
                            else
                            {
                                _logger.LogInformation("\nPayment Controller Unregistered: Response Validation UnSuccessfull for Unregistered Payment\n");
                                DPCErrorModel errors = JsonConvert.DeserializeObject<DPCErrorModel>(_validationResponseContent);
                                return StatusCode(422, errors);
                            }
                        }
                        else if (responseKind.Equals("card"))
                        {


                            _unregPaymentResValidation = await _utilityService.ValidateInboundRequest(_unregPaymentResponseContent, _paymentCardResponseUnregSchemaName, vendorCode, lobCode);
                            _validationResponseContent = await _unregPaymentResValidation.Content.ReadAsStringAsync();
                            if (_unregPaymentResValidation.StatusCode == HttpStatusCode.OK)
                            {
                                _logger.LogInformation("\nPayment Controller Unregistered: Card Response Validation Successfull for Unregistered Payment\n");
                                Unreg_Card_Payment_Response_DTO responseCardDto = JsonConvert.DeserializeObject<Unreg_Card_Payment_Response_DTO>(_unregPaymentResponseContent);
                                return new ObjectResult(responseCardDto);
                            }
                            else
                            {
                                _logger.LogInformation("\nPayment Controller Unregistered: Card Response Validation Unsuccessfull for Unregistered Payment\n");
                                DPCErrorModel errors = JsonConvert.DeserializeObject<DPCErrorModel>(_validationResponseContent);
                                return StatusCode(422, errors);
                            }
                        }
                        else
                        {
                            throw new NotAcceptableException(vendorCode, lobCode);
                        }


                    }
                    else if (_unregPaymentResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                    {
                        _unregPaymentResponseContent = await _unregPaymentResponse.Content.ReadAsStringAsync();

                       
                        DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(_unregPaymentResponseContent);
                        var formatedRes = new ObjectResult(errorDetails);
                        formatedRes.StatusCode = errorDetails.error.status;
                        return formatedRes;
                    }
                    else
                    {
                        throw new BadRequestException(vendorCode, lobCode);

                    }


                }
                else
                {
                    //Need to implement Validation Error
                    _validationResponseContent = await _unregPaymentReqValidation.Content.ReadAsStringAsync();
                    DPCErrorModel errorContent = JsonConvert.DeserializeObject<DPCErrorModel>(_validationResponseContent);
                    var errorResponse = new ObjectResult(errorContent);
                    errorResponse.StatusCode = errorContent.error.status;
                    return errorResponse;

                }

            }
            catch (HttpRequestException ex)
            {
                throw new ServiceNotAvailableException(vendorCode, lobCode);
            }
            catch (JsonReaderException ex)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        #endregion



    }
}