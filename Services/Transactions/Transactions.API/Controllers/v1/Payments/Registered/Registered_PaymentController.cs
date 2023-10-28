using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Transaction.API.Application.ModelDTOs.v1.Payment.Request.Registered;
using Transaction.API.Application.ModelDTOs.v1.Payment.Response.Registered;
using Transaction.API.Application.Services.v1.Services.Abstraction;
using Transaction.API.Domain.Entities;
using Transaction.API.Domain.Exceptions;
using Transactions.API.Domain.Exceptions;

namespace Transaction.API.Controllers
{
    [ApiController]    
    [ApiVersion("1")]
    public class Registered_PaymentController : ControllerBase
    {
        #region Payment Controller Variables
        private readonly IUtilitiesService _utilityService;
        private readonly IRegisteredPaymentService _paymentService;
        private readonly ILogger<Registered_PaymentController> _logger;
        private readonly string _schemaName;
        private AppSettings _appSettings { get; }

   
    
        private readonly string _regPaymentSchemaName = "Payment/Payment_Request_Schema_Registered.json";
        private readonly string _paymentCardResponseRegSchemaName = "Payment/Payment_Card_Response_Schema_Registered.json";
        private readonly string _paymentAchResponseRegSchemaName = "Payment/Payment_Ach_Response_Schema_Registered.json";
        private HttpResponseMessage _regPaymentReqValidation;
        private string _regPaymentReqValidationContent;
        private HttpResponseMessage _regPaymentResValidation;
        private string _regPaymentResValidationContent;
        private HttpResponseMessage _regPaymentResponse;
        private string _regPaymentResponseContent;
        #endregion
       

        public Registered_PaymentController(IUtilitiesService utilityService, IRegisteredPaymentService paymentService, ILogger<Registered_PaymentController> logger, IOptionsSnapshot<AppSettings> appSettings)
        {
            _utilityService = utilityService;
            _paymentService = paymentService;
            _logger = logger;
            _appSettings = appSettings.Value;
            _schemaName = _appSettings.SchemaName;

        }
        

        #region Payment Registered User

        /// <summary>
        /// Get the payment request, validate the request if it is validated then pass this request to Payment Service class.
        /// Get the Response from Payment Service class, validate the response if it is validated return the response.
        /// In case of errors and unsuccessful validation return proper message.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/v{version:apiVersion}/registered/payments")]
        //[Route("api/[controller]/unregistered/payment")]
        public async Task<IActionResult> PaymentRegisteredUser()
        {
            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];

            try
            {
                _logger.LogInformation("\nPayment Microservice called for REGISTERED USER.\n");

                // Get header values
               
                string paymentJsonBody = string.Empty;
                var reader = new StreamReader(Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false);
                paymentJsonBody = await reader.ReadToEndAsync();

                if (paymentJsonBody == null)
                {
                    throw new BadRequestException(vendorCode, lobCode);
                }

                JObject data = JObject.Parse(paymentJsonBody);

                _regPaymentReqValidation = await _utilityService.ValidateInboundRequest(paymentJsonBody, _regPaymentSchemaName, vendorCode, lobCode);
               

                if (_regPaymentReqValidation.StatusCode == HttpStatusCode.OK)
                {

                    _logger.LogInformation("\nPayment Controller Registered: Request Validation Successfull for Registered Payment, now will start processing payment\n");
                    _regPaymentReqValidationContent = await _regPaymentReqValidation.Content.ReadAsStringAsync();
                    Reg_Payment_Request_DTO requestDto = JsonConvert.DeserializeObject<Reg_Payment_Request_DTO>(paymentJsonBody);

                    _regPaymentResponse = await _paymentService.MakeRegisteredPayment(requestDto, vendorCode, lobCode);

                    _regPaymentResponseContent = await _regPaymentResponse.Content.ReadAsStringAsync();
                    if(_regPaymentResponseContent == null)
                    {
                        throw new NoContentException(vendorCode,lobCode);
                    }
                   
                    if (_regPaymentResponse.StatusCode == HttpStatusCode.OK)
                    {
                        JObject aciPaymentResponseJsonBody = JObject.Parse(_regPaymentResponseContent);
                        _logger.LogInformation("\nPayment Controller Registered: Response Fetched Successfull. Now will proceed for the response validation.\n");

                        bool kindExist = aciPaymentResponseJsonBody.SelectTokens("fundingAccountSummary.kind").Any();
                        if (!kindExist)
                        {
                            throw new NotAcceptableException(vendorCode, lobCode);
                        }
                        string responseKind = aciPaymentResponseJsonBody.SelectToken("fundingAccountSummary.kind").ToString();
                        responseKind = responseKind.ToLower();
                        if (responseKind.Equals("ach"))
                        {


                            _regPaymentResValidation = await _utilityService.ValidateInboundRequest(_regPaymentResponseContent, _paymentAchResponseRegSchemaName, vendorCode, lobCode);
                            _regPaymentResValidationContent = await _regPaymentResValidation.Content.ReadAsStringAsync();
                            if (_regPaymentResValidation.StatusCode == HttpStatusCode.OK)
                            {
                                _logger.LogInformation("\nPayment Controller Registered: Ach Response Validation Successfull for Registered Payment\n");
                                Reg_Ach_Payment_Response_DTO responseAchDto = JsonConvert.DeserializeObject<Reg_Ach_Payment_Response_DTO>(_regPaymentResponseContent);
                                return new ObjectResult(responseAchDto);
                            }
                            else
                            {
                                _logger.LogInformation("\nPayment Controller Unregistered: Response Validation UnSuccessfull for Unregistered Payment\n");
                                DPCErrorModel errors = JsonConvert.DeserializeObject<DPCErrorModel>(_regPaymentResValidationContent);
                                return StatusCode(422, errors);
                            }
                        }
                        else if(responseKind.Equals("card"))
                        {

                            _regPaymentResValidation = await _utilityService.ValidateInboundRequest(_regPaymentResponseContent, _paymentCardResponseRegSchemaName, vendorCode, lobCode);
                            _regPaymentResValidationContent = await _regPaymentResValidation.Content.ReadAsStringAsync();
                            if (_regPaymentResValidation.StatusCode == HttpStatusCode.OK)
                            {
                                _logger.LogInformation("\nPayment Controller Registered: Card Response Validation Successfull for Registered Payment\n");
                                Reg_Card_Payment_Response_DTO responseCardDto = JsonConvert.DeserializeObject<Reg_Card_Payment_Response_DTO>(_regPaymentResponseContent);
                                return new ObjectResult(responseCardDto);
                            }
                            else
                            {
                                _logger.LogInformation("\nPayment Controller Registered: Card Response Validation Unsuccessfull for Registered Payment\n");
                                var errors = JsonConvert.DeserializeObject<RootError>(_regPaymentResValidationContent);
                                return StatusCode(422, errors);
                            }
                        }
                         else
                        {
                            throw new NotAcceptableException(vendorCode, lobCode);
                        }

                    }
                    else if (_regPaymentResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                    {
                        _regPaymentResponseContent = await _regPaymentResponse.Content.ReadAsStringAsync();


                        DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(_regPaymentResponseContent);
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
                    _logger.LogInformation("\nPayment Controller Registered: Request Validation Unsuccessfull for Registered Payment\n");
                    _regPaymentReqValidationContent = await _regPaymentReqValidation.Content.ReadAsStringAsync();
                    DPCErrorModel errors = JsonConvert.DeserializeObject<DPCErrorModel>(_regPaymentReqValidationContent);
                    return StatusCode(422, errors);

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