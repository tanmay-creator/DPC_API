
using Polly.CircuitBreaker;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using VendorConnect.API.Application.ModelDTOs.FundingAccount.Request.Unregistered;
using VendorConnect.API.Application.ModelDTOs.FundingAccount.Response;
using VendorConnect.API.Application.ModelDTOs.FundingAccount.Response.Unregistered;
using VendorConnect.API.Application.ModelDTOs.Payment.Request.Registered;
using VendorConnect.API.Application.ModelDTOs.Payment.Response.Registered;
using VendorConnect.API.Application.ModelDTOs.v1.FundingAccount.Response.Registered;
using VendorConnect.API.Application.Services.Services.Abstraction;
using VendorConnect.API.Domain.Entities;
using VendorConnect.API.Domain.Exceptions;

namespace VendorConnect.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiVersion("1.0")]
    public class VendorConnectController : ControllerBase
    {

        #region PaymentVariables
        private HttpResponseMessage _paymentReqValidation;
        private HttpResponseMessage _paymentResValidation;
        private string _paymentResValidationContent;
        private string _paymentReqValidationContent;

        #region Registered
        private const string _paymentRegRequestSchemaName = "Payment/Payment_Request_Schema_Registered.json";
        private const string _paymentRegCardResponseSchemaName = "Payment/Payment_Card_Response_Schema_Registered.json";
        private const string _paymentRegAchResponseSchemaName = "Payment/Payment_Ach_Response_Schema_Registered.json";
        private HttpResponseMessage _paymentRegServiceResponse;
        private string _paymentRegServiceResponseContent;

        private Reg_Card_Payment_Response_DTO reg_card_res = null;
        private Reg_Ach_Payment_Response_DTO reg_ach_res = null;
        #endregion

        #region Unregistered
        private const string _paymentUnregRequestSchemaName = "Payment/Payment_Request_Schema_Unregistered.json";
        private const string _paymentUnregCardResponseSchemaName = "Payment/Payment_Card_Response_Schema_Unregistered.json";
        private const string _paymentUnregAchResponseSchemaName = "Payment/Payment_Ach_Response_Schema_Unregistered.json";

        private Unreg_Card_Payment_Response_DTO unreg_card_res = null;
        private Unreg_Ach_Payment_Response_DTO unreg_ach_res = null;

        //private const string _paymentUnregRequestSchemaName = "Payment/Payment_Reg_Unreg_OneOf_Schema.json";
        //private const string _paymentUnregCardResponseSchemaName = "Payment/Payment_Reg_Unreg_OneOf_Schema.json";
        //private const string _paymentUnregAchResponseSchemaName = "Payment/Payment_Reg_Unreg_OneOf_Schema.json";
        private HttpResponseMessage _paymentUnregServiceResponse;
        private string _paymentUnregServiceResponseContent;
        #endregion

        #endregion



        private IVendorConnectService _vendorConnectService;
        private IUtilityService _utilityService;
        private readonly ILogger<VendorConnectController> _logger;
        public string RequestSchemaName;
        public string ResponseSchemaName;
        public HttpResponseMessage RequestValidation;
        public HttpResponseMessage ResponseValidation;
        public string RequestValidationContent;
        public string ResponseValidationContent;
        public HttpResponseMessage AciResponse;
        public string AciResponseContent;
        public ObjectResult ErrorResponse;
        public RootError ValidationErrors;
        public readonly string GetAchFAResponseSchema = "FundingAccount/FundingAccount_RegACH_ResponseSchema.json";
        public readonly string GetCardFAResponseSchema = "FundingAccount/FundingAccount_RegCard_ResponseSchema.json";

        public VendorConnectController(IVendorConnectService vendorConnectService, ILogger<VendorConnectController> logger, IUtilityService utilityService)
        {
            _vendorConnectService = vendorConnectService;
            _utilityService = utilityService;
            _logger = logger;

            _logger.LogInformation("VendorConnectController:: VendorConnectController");
            _utilityService = utilityService;
        }


        #region Funding Account Controllers

        [HttpGet]
        [Route("api/v{version:apiVersion}/registered/{profile-id}/fundingaccount")]
        public async Task<IActionResult> GetAllFundingAccount([FromRoute(Name = "profile-id")] string profileId)
        {
            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];
            try
            {
                _logger.LogInformation("\nVendorConnect Controller: Get All Funding Account Method called for REGISTERED USER.\n");
                AciResponse = await _vendorConnectService.GetAllFundingAccount(profileId, vendorCode, lobCode);
                if (AciResponse.StatusCode == HttpStatusCode.OK)
                {

                    AciResponseContent = await AciResponse.Content.ReadAsStringAsync();

                    if (AciResponseContent == null)
                    {
                        throw new NoContentException(vendorCode, lobCode);
                    }

                    Reg_GetAllFA_ResponseDTO reg_GetAllFA_ResponseDTO = JsonConvert.DeserializeObject<Reg_GetAllFA_ResponseDTO>(AciResponseContent);
                    var response = new ObjectResult(reg_GetAllFA_ResponseDTO);
                    response.StatusCode = (int)AciResponse.StatusCode;
                    return response;

                }
                else if (AciResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                {
                    AciResponseContent = await AciResponse.Content.ReadAsStringAsync();


                    DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(AciResponseContent);
                    var formatedRes = new ObjectResult(errorDetails);
                    formatedRes.StatusCode = errorDetails.error.status;
                    return formatedRes;
                }
                else
                {
                    throw new BadRequestException(vendorCode, lobCode);
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
            catch (BrokenCircuitException e)
            {
                throw new CircuitBreakerException(vendorCode, lobCode);
            }
            catch (Exception e)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }
        }

        [HttpGet]
        [Route("api/v{version:apiVersion}/registered/{profile-id}/fundingaccount/{fundingAccount-id}")]
        public async Task<IActionResult> GetFundingAccount([FromRoute(Name = "profile-id")] string profileId, [FromRoute(Name = "fundingAccount-id")] string fundingAccountId)
        {
            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];
            try
            {
                _logger.LogInformation("\nVendorConnect Controller: Get Funding Account Method called for REGISTERED USER.\n");
                AciResponse = await _vendorConnectService.GetFundingAccount(profileId, fundingAccountId, vendorCode, lobCode);

                if (AciResponse.StatusCode == HttpStatusCode.OK)
                {
                    AciResponseContent = await AciResponse.Content.ReadAsStringAsync();

                    if (AciResponseContent == null)
                    {
                        throw new NoContentException(vendorCode, lobCode);
                    }
                    JObject getFAJsonBody = JObject.Parse(AciResponseContent);
                    _logger.LogInformation("\nVendorConnect Controller: Response Fetched Successfull. Now will proceed for the response validation.\n");

                    bool kindExist = getFAJsonBody.SelectTokens("kind").Any();
                    if (!kindExist)
                    {
                        throw new NotAcceptableException(vendorCode, lobCode);
                    }
                    string responseKind = getFAJsonBody.SelectToken("kind").ToString();
                    responseKind = responseKind.ToLower();
                    if (responseKind.Equals("ach"))
                    {


                        ResponseValidation = await _utilityService.ValidateRequestResponse(AciResponseContent, GetAchFAResponseSchema, vendorCode, lobCode);
                        ResponseValidationContent = await ResponseValidation.Content.ReadAsStringAsync();
                        if (ResponseValidation.StatusCode == HttpStatusCode.OK)
                        {
                            _logger.LogInformation("\nVendorConnect Controller: Ach Response Validation Successfull for this funding account and user profile.\n");
                            Reg_GetFA_Ach_Response_DTO responseAchDto = JsonConvert.DeserializeObject<Reg_GetFA_Ach_Response_DTO>(AciResponseContent);
                            return new ObjectResult(responseAchDto);
                        }
                        else
                        {
                            _logger.LogInformation("\nVendorConnect Controller: Response Validation UnSuccessfull for this funding account and user profile\n");
                            DPCErrorModel errors = JsonConvert.DeserializeObject<DPCErrorModel>(ResponseValidationContent);
                            return StatusCode(422, errors);
                        }
                    }
                    else if (responseKind.Equals("card"))
                    {

                        ResponseValidation = await _utilityService.ValidateRequestResponse(AciResponseContent, GetCardFAResponseSchema, vendorCode, lobCode);
                        ResponseValidationContent = await ResponseValidation.Content.ReadAsStringAsync();
                        if (ResponseValidation.StatusCode == HttpStatusCode.OK)
                        {
                            _logger.LogInformation("\nVendorConnect Controller: Card Response Validation Successfull for this funding account and user profile\n");
                            Reg_GetFA_Card_Response_DTO responseCardDto = JsonConvert.DeserializeObject<Reg_GetFA_Card_Response_DTO>(AciResponseContent);
                            return new ObjectResult(responseCardDto);
                        }
                        else
                        {
                            _logger.LogInformation("\nPayment Controller Registered: Card Response Validation Unsuccessfull for this funding account and user profile\n");
                            DPCErrorModel errors = JsonConvert.DeserializeObject<DPCErrorModel>(ResponseValidationContent);
                            return StatusCode(422, errors);
                        }
                    }
                    else
                    {
                        throw new NotAcceptableException(vendorCode, lobCode);
                    }

                }
                else if (AciResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                {
                    AciResponseContent = await AciResponse.Content.ReadAsStringAsync();


                    DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(AciResponseContent);
                    var formatedRes = new ObjectResult(errorDetails);
                    formatedRes.StatusCode = errorDetails.error.status;
                    return formatedRes;
                }
                else
                {
                    throw new BadRequestException(vendorCode, lobCode);

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
            catch (BrokenCircuitException e)
            {
                throw new CircuitBreakerException(vendorCode, lobCode);
            }
            catch (Exception e)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }
        }

        [HttpPost]
        //[Route("CreateUnregisteredCardFundingAccount")]
        [Route("api/v{version:apiVersion}/unregistered/cardfundingaccount")]
        public async Task<IActionResult> CreateUnregisteredCardFundingAccount([FromBody] Unreg_FA_Card_Request_DTO unreg_FA_Card_Request_DTO)
        {
            // Request validation
            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];
            try
            {
                RequestSchemaName = "FundingAccount/FundingAccount_UnregCard_RequestSchema.json";
                string aciCardRequest = JsonConvert.SerializeObject(unreg_FA_Card_Request_DTO);

                RequestValidation = await _utilityService.ValidateRequestResponse(aciCardRequest, RequestSchemaName, vendorCode, lobCode);
                RequestValidationContent = await RequestValidation.Content.ReadAsStringAsync();

                if (RequestValidation.StatusCode == HttpStatusCode.OK)
                {
                    AciResponse = await _vendorConnectService.UnregisteredCardFundingAccount(unreg_FA_Card_Request_DTO);
                    AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                    if (AciResponse.StatusCode == HttpStatusCode.Created)
                    {
                        // Response validation
                        ResponseSchemaName = "FundingAccount/FundingAccount_UnregCard_ResponseSchema.json";
                        //string aciCardResponse = await AciResponse.Content.ReadAsStringAsync();
                        ResponseValidation = await _utilityService.ValidateRequestResponse(AciResponseContent, ResponseSchemaName, vendorCode, lobCode);
                        ResponseValidationContent = await ResponseValidation.Content.ReadAsStringAsync();
                        if (ResponseValidation.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            //throw new UnauthorizedException -- DPC created class
                        }

                        if (ResponseValidation.StatusCode == HttpStatusCode.OK)
                        {
                            Unreg_FA_Card_Response_DTO unreg_FA_Card = JsonConvert.DeserializeObject<Unreg_FA_Card_Response_DTO>(AciResponseContent);
                            return new ObjectResult(unreg_FA_Card);
                        }
                        else if (ResponseValidation.StatusCode == HttpStatusCode.UnprocessableEntity)
                        {
                            ValidationErrors = JsonConvert.DeserializeObject<RootError>(ResponseValidationContent);
                            return StatusCode(422, ValidationErrors);
                        }

                        throw new BadRequestException(vendorCode, lobCode);
                    }
                    else if (AciResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                    {

                        AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                        HttpResponseMessage formatedErrorDetails = await _utilityService.GetFormatedErrors(AciResponseContent, vendorCode, lobCode);
                        string content = await formatedErrorDetails.Content.ReadAsStringAsync();
                        DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(content);
                        var formatedRes = new ObjectResult(errorDetails);
                        formatedRes.StatusCode = errorDetails.error.status;
                        return formatedRes;
                    }
                    else
                    {
                        throw new VendorException(vendorCode, lobCode, _paymentUnregServiceResponse.StatusCode);
                    }
                }
                else
                {
                    ValidationErrors = JsonConvert.DeserializeObject<RootError>(RequestValidationContent);
                    return StatusCode(422, ValidationErrors);
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
            catch (BrokenCircuitException e)
            {
                throw new CircuitBreakerException(vendorCode, lobCode);
            }
            catch (Exception e)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }

        }

        [HttpPost]
        //[Route("CreateUnregisteredACHFundingAccount")]
        [Route("api/v{version:apiVersion}/unregistered/achfundingaccount")]
        public async Task<IActionResult> CreateUnregisteredACHFundingAccount([FromBody] Unreg_FA_ACH_Request_DTO unreg_FA_ACH_Request_DTO)
        {

            _logger.LogInformation("VendorConnectController:: Create unregistered card funding account in ACI for the user ");

            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];
            try
            {
                // Request validation     FundingAccount_UnregACH_RequestSchema   //FundingAccount_RegUnreg_RequestSchema.json
                RequestSchemaName = "FundingAccount/FundingAccount_UnregACH_RequestSchema.json";
                string aciACHRequest = JsonConvert.SerializeObject(unreg_FA_ACH_Request_DTO);
                RequestValidation = await _utilityService.ValidateRequestResponse(aciACHRequest, RequestSchemaName, vendorCode, lobCode);
                RequestValidationContent = await RequestValidation.Content.ReadAsStringAsync();

                if (RequestValidation.StatusCode == HttpStatusCode.OK)
                {
                    AciResponse = await _vendorConnectService.UnregisteredACHFundingAccount(unreg_FA_ACH_Request_DTO);
                    AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                    if (AciResponse.StatusCode == HttpStatusCode.Created)
                    {
                        // Response validation                //FundingAccount_UnregACH_ResponseSchema.json   //FundingAccountACHResponseSchema
                        ResponseSchemaName = "FundingAccount/FundingAccount_UnregACH_ResponseSchema.json";
                        ResponseValidation = await _utilityService.ValidateRequestResponse(AciResponseContent, ResponseSchemaName, vendorCode, lobCode);
                        ResponseValidationContent = await ResponseValidation.Content.ReadAsStringAsync();

                        if (ResponseValidation.StatusCode == HttpStatusCode.OK)
                        {
                            Unreg_FA_ACH_Response_DTO unreg_FA_ACH = JsonConvert.DeserializeObject<Unreg_FA_ACH_Response_DTO>(AciResponseContent);
                            return new ObjectResult(unreg_FA_ACH);
                        }
                        else if (ResponseValidation.StatusCode == HttpStatusCode.UnprocessableEntity)
                        {
                            ValidationErrors = JsonConvert.DeserializeObject<RootError>(ResponseValidationContent);
                            return StatusCode(422, ValidationErrors);
                        }
                        throw new BadRequestException(vendorCode, lobCode);
                    }
                    else if (AciResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                    {

                        AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                        HttpResponseMessage formatedErrorDetails = await _utilityService.GetFormatedErrors(AciResponseContent, vendorCode, lobCode);
                        string content = await formatedErrorDetails.Content.ReadAsStringAsync();
                        DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(content);
                        var formatedRes = new ObjectResult(errorDetails);
                        formatedRes.StatusCode = errorDetails.error.status;
                        return formatedRes;
                    }
                    else
                    {
                        throw new VendorException(vendorCode, lobCode, _paymentUnregServiceResponse.StatusCode);
                    }
                }
                else
                {
                    ValidationErrors = JsonConvert.DeserializeObject<RootError>(RequestValidationContent);
                    return StatusCode(422, ValidationErrors);
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
            catch (BrokenCircuitException e)
            {
                throw new CircuitBreakerException(vendorCode, lobCode);
            }
            catch (Exception e)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }

        }


        // Registered flow

        [HttpPost]
        //[Route("CreateRegisteredCardFundingAccount")]
        [Route("api/v{version:apiVersion}/registered/cardfundingaccount")]
        public async Task<IActionResult> CreateRegisteredCardFundingAccount([FromBody] Reg_FA_Card_Request_DTO reg_FA_Card_Request_DTO)
        {

            _logger.LogInformation("VendorConnectController:: Create unregistered card funding account in ACI for the user ");

            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];
            try
            {
                // Request validation
                RequestSchemaName = "FundingAccount/FundingAccount_RegCard_RequestSchema.json";
                string aciCardRequest = JsonConvert.SerializeObject(reg_FA_Card_Request_DTO);
                RequestValidation = await _utilityService.ValidateRequestResponse(aciCardRequest, RequestSchemaName, vendorCode, lobCode);
                RequestValidationContent = await RequestValidation.Content.ReadAsStringAsync();

                if (RequestValidation.StatusCode == HttpStatusCode.OK)
                {
                    AciResponse = await _vendorConnectService.RegisteredCardFundingAccount(reg_FA_Card_Request_DTO);
                    AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                    if (AciResponse.StatusCode == HttpStatusCode.Created)
                    {
                        // Response validation
                        ResponseSchemaName = "FundingAccount/FundingAccount_RegCard_ResponseSchema.json";
                        ResponseValidation = await _utilityService.ValidateRequestResponse(AciResponseContent, ResponseSchemaName, vendorCode, lobCode);
                        ResponseValidationContent = await ResponseValidation.Content.ReadAsStringAsync();
                        if (ResponseValidation.StatusCode == HttpStatusCode.OK)
                        {
                            Reg_FA_Card_Response_DTO reg_FA_Card = JsonConvert.DeserializeObject<Reg_FA_Card_Response_DTO>(AciResponseContent);
                            return new ObjectResult(reg_FA_Card);
                        }
                        else if (ResponseValidation.StatusCode == HttpStatusCode.UnprocessableEntity)
                        {
                            ValidationErrors = JsonConvert.DeserializeObject<RootError>(ResponseValidationContent);
                            return StatusCode(422, ValidationErrors);
                        }
                        throw new BadRequestException(vendorCode, lobCode);
                    }
                    else if (AciResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                    {

                        AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                        HttpResponseMessage formatedErrorDetails = await _utilityService.GetFormatedErrors(AciResponseContent, vendorCode, lobCode);
                        string content = await formatedErrorDetails.Content.ReadAsStringAsync();
                        DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(content);
                        var formatedRes = new ObjectResult(errorDetails);
                        formatedRes.StatusCode = errorDetails.error.status;
                        return formatedRes;
                    }
                    else
                    {
                        throw new VendorException(vendorCode, lobCode, _paymentUnregServiceResponse.StatusCode);
                    }
                }
                else
                {
                    ValidationErrors = JsonConvert.DeserializeObject<RootError>(RequestValidationContent);
                    return StatusCode(422, ValidationErrors);
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
            catch (BrokenCircuitException e)
            {
                throw new CircuitBreakerException(vendorCode, lobCode);
            }
            catch (Exception e)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }
        }

        [HttpPost]
        //[Route("CreateRegisteredACHFundingAccount")]
        [Route("api/v{version:apiVersion}/registered/achfundingaccount")]
        public async Task<IActionResult> CreateRegisteredACHFundingAccount([FromBody] Reg_FA_ACH_Request_DTO reg_FA_ACH_Request_DTO)
        {

            _logger.LogInformation("VendorConnectController:: Create unregistered card funding account in ACI for the user ");

            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];
            try
            {
                // Request validation
                RequestSchemaName = "FundingAccount/FundingAccount_RegACH_RequestSchema.json";
                string aciACHRequest = JsonConvert.SerializeObject(reg_FA_ACH_Request_DTO);
                RequestValidation = await _utilityService.ValidateRequestResponse(aciACHRequest, RequestSchemaName, vendorCode, lobCode);
                RequestValidationContent = await RequestValidation.Content.ReadAsStringAsync();

                if (RequestValidation.StatusCode == HttpStatusCode.OK)
                {
                    AciResponse = await _vendorConnectService.RegisteredACHFundingAccount(reg_FA_ACH_Request_DTO);
                    AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                    if (AciResponse.StatusCode == HttpStatusCode.Created)
                    {
                        // Response validation
                        string responseSchemaName = "FundingAccount/FundingAccount_RegACH_ResponseSchema.json";
                        ResponseValidation = await _utilityService.ValidateRequestResponse(AciResponseContent, responseSchemaName, vendorCode, lobCode);
                        ResponseValidationContent = await ResponseValidation.Content.ReadAsStringAsync();
                        if (ResponseValidation.StatusCode == HttpStatusCode.OK)
                        {
                            Reg_FA_ACH_Response_DTO reg_FA_ACH = JsonConvert.DeserializeObject<Reg_FA_ACH_Response_DTO>(AciResponseContent);
                            return new ObjectResult(reg_FA_ACH);
                        }
                        else if (ResponseValidation.StatusCode == HttpStatusCode.UnprocessableEntity)
                        {
                            ValidationErrors = JsonConvert.DeserializeObject<RootError>(ResponseValidationContent);
                            return StatusCode(422, ValidationErrors);
                        }
                        throw new BadRequestException(vendorCode, lobCode);
                    }
                    else if (AciResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                    {

                        AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                        HttpResponseMessage formatedErrorDetails = await _utilityService.GetFormatedErrors(AciResponseContent, vendorCode, lobCode);
                        string content = await formatedErrorDetails.Content.ReadAsStringAsync();
                        DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(content);
                        var formatedRes = new ObjectResult(errorDetails);
                        formatedRes.StatusCode = errorDetails.error.status;
                        return formatedRes;
                    }
                    else
                    {
                        throw new VendorException(vendorCode, lobCode, AciResponse.StatusCode);
                    }
                }
                else
                {
                    ValidationErrors = JsonConvert.DeserializeObject<RootError>(RequestValidationContent);
                    return StatusCode(422, ValidationErrors);
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
            catch (BrokenCircuitException e)
            {
                throw new CircuitBreakerException(vendorCode, lobCode);
            }
            catch (Exception e)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }
        }

        // Delete Funding Account
        [HttpDelete]
        [Route("{billerId}/{billerAccountId}/DeleteUnregFundingAccount/{fundingAccountId}")]
        //[Route("api/v{version:apiVersion}/unregistered/fundingaccount/{billerId}/{billerAccountId}/{fundingAccountId}")]
        public async Task<IActionResult> DeleteUnregisteredFundingAccount(string billerId, string billerAccountId, string fundingAccountId)
        {
            _logger.LogInformation("VendorConnectController:: Delete funding account for unregistered user");
            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];
            try
            {
                AciResponse = await _vendorConnectService.DeleteUnregisteredFundingAccount(billerId, billerAccountId, fundingAccountId);
                AciResponseContent = await AciResponse.Content.ReadAsStringAsync();

                if (AciResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    //return new ObjectResult(AciResponse);
                    ObjectResult deleteResponse = new ObjectResult(AciResponseContent);
                    deleteResponse.StatusCode = (int)AciResponse.StatusCode;
                    return deleteResponse;
                }
                else if (AciResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                {

                    AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                    HttpResponseMessage formatedErrorDetails = await _utilityService.GetFormatedErrors(AciResponseContent, vendorCode, lobCode);
                    string content = await formatedErrorDetails.Content.ReadAsStringAsync();
                    DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(content);
                    var formatedRes = new ObjectResult(errorDetails);
                    formatedRes.StatusCode = errorDetails.error.status;
                    return formatedRes;
                }
                else
                {
                    throw new VendorException(vendorCode, lobCode, AciResponse.StatusCode);
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
            catch (BrokenCircuitException e)
            {
                throw new CircuitBreakerException(vendorCode, lobCode);
            }
            catch (Exception e)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }
        }

        [HttpDelete]
        [Route("{profileId}/DeleteRegFundingAccount/{fundingAccountId}")]
        //[Route("api/v{version:apiVersion}/registered/fundingaccount/{fundingAccountId}")]
        public async Task<IActionResult> DeleteRegisteredFundingAccount(string profileId, string fundingAccountId)
        {
            _logger.LogInformation("VendorConnectController:: Delete funding account for unregistered user");
            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];
            try
            {
                AciResponse = await _vendorConnectService.DeleteRegisteredFundingAccount(profileId, fundingAccountId);
                AciResponseContent = await AciResponse.Content.ReadAsStringAsync();

                if (AciResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    //return new ObjectResult(AciResponse);
                    ObjectResult deleteResponse = new ObjectResult(AciResponseContent);
                    deleteResponse.StatusCode = (int)AciResponse.StatusCode;
                    return deleteResponse;
                }
                else if (AciResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                {

                    AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                    HttpResponseMessage formatedErrorDetails = await _utilityService.GetFormatedErrors(AciResponseContent, vendorCode, lobCode);
                    string content = await formatedErrorDetails.Content.ReadAsStringAsync();
                    DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(content);
                    var formatedRes = new ObjectResult(errorDetails);
                    formatedRes.StatusCode = errorDetails.error.status;
                    return formatedRes;
                }
                else
                {
                    throw new VendorException(vendorCode, lobCode, AciResponse.StatusCode);
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
            catch (BrokenCircuitException e)
            {
                throw new CircuitBreakerException(vendorCode, lobCode);
            }
            catch (Exception e)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }
        }
        #endregion

        #region Payment Controller

        // Make payment flow

        /// <summary>
        /// Get a payment request from Payment Microservice and validate the request if validated
        /// then pass this request to Vendor Connect Service to make payment for registered user.
        /// Get the response from service method validate the response if validated then pass the response to Payment Microservice.
        /// In case of error and validation failure return the proper error message.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("RegisteredPaymentRequestToVendorConnect")]
        //[Route("api/v{version:apiVersion}/registered/payment")]
        public async Task<IActionResult> RegisteredPaymentRequestToVendorConnect()
        {

            string SCHEMA_NAME = string.Empty;

            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];

            try
            {
                string paymentRequestJsonBody = string.Empty;
                var reader = new StreamReader(Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false);
                paymentRequestJsonBody = await reader.ReadToEndAsync();

                if (paymentRequestJsonBody == null)
                {
                    throw new BadRequestException(vendorCode, lobCode);
                }
                var yourObject = System.Text.Json.JsonDocument.Parse(paymentRequestJsonBody);
                _logger.LogInformation("\nVendorConnect Controller:: Start Payment Processing in ACI.\n");


                _paymentReqValidation = await _utilityService.ValidateRequestResponse(paymentRequestJsonBody, _paymentRegRequestSchemaName, vendorCode, lobCode);
                _paymentReqValidationContent = await _paymentReqValidation.Content.ReadAsStringAsync();
                if (_paymentReqValidation.StatusCode == HttpStatusCode.OK)
                {

                    _logger.LogInformation("\nVendorConnect Controller: Request Validation Successfull for Unregistered User.\n");
                    Reg_Payment_Request_DTO requestDto = JsonConvert.DeserializeObject<Reg_Payment_Request_DTO>(paymentRequestJsonBody);
                    _paymentRegServiceResponse = await _vendorConnectService.RegisteredPayment(requestDto);


                    if (_paymentRegServiceResponse.StatusCode == HttpStatusCode.OK)
                    {
                        _logger.LogInformation("\nVendorConnect Controller: Response Fetched Successfull for Unregistered User.\n");

                        _paymentRegServiceResponseContent = await _paymentRegServiceResponse.Content.ReadAsStringAsync();
                        if (_paymentRegServiceResponseContent == null)
                        {
                            throw new NoContentException(vendorCode, lobCode);
                        }
                        JObject aciPaymentResponseJsonBody = JObject.Parse(_paymentRegServiceResponseContent);

                        bool kindExist = aciPaymentResponseJsonBody.SelectTokens("fundingAccountSummary.kind").Any();
                        if (!kindExist)
                        {
                            throw new NotAcceptableException(vendorCode, lobCode);
                        }
                        string responseKind = aciPaymentResponseJsonBody.SelectToken("fundingAccountSummary.kind").ToString();
                        responseKind = responseKind.ToLower();

                        if (responseKind.Equals("ach"))
                        {


                            _paymentResValidation = await _utilityService.ValidateRequestResponse(_paymentRegServiceResponseContent, _paymentRegAchResponseSchemaName, vendorCode, lobCode);
                            _paymentResValidationContent = await _paymentResValidation.Content.ReadAsStringAsync();
                            if (_paymentResValidation.StatusCode != HttpStatusCode.OK)
                            {
                                _logger.LogInformation("\nVendorConnect Controller: Response Validated Unsuccessfull for Unregistered User {0}.\n", responseKind);
                                DPCErrorModel errors = JsonConvert.DeserializeObject<DPCErrorModel>(_paymentResValidationContent);
                                return StatusCode(422, errors);

                            }
                            _logger.LogInformation("\nVendorConnect Controller: Response Fetched Successfull for Unregistered User {0}.\n", responseKind);
                            reg_ach_res = JsonConvert.DeserializeObject<Reg_Ach_Payment_Response_DTO>(_paymentRegServiceResponseContent);
                            return new ObjectResult(reg_ach_res);


                        }
                        else
                        {

                            _paymentResValidation = await _utilityService.ValidateRequestResponse(_paymentRegServiceResponseContent, _paymentRegCardResponseSchemaName, vendorCode, lobCode);
                            _paymentResValidationContent = await _paymentResValidation.Content.ReadAsStringAsync();
                            if (_paymentResValidation.StatusCode != HttpStatusCode.OK)
                            {
                                _logger.LogInformation("\nVendorConnect Controller: Response Validated Unsuccessfull for Unregistered User {0}.\n", responseKind);
                                DPCErrorModel errors = JsonConvert.DeserializeObject<DPCErrorModel>(_paymentResValidationContent);
                                return StatusCode(422, errors);
                            }
                            _logger.LogInformation("\nVendorConnect Controller: Response Fetched Successfull for Unregistered User {0}.\n", responseKind);
                            reg_card_res = JsonConvert.DeserializeObject<Reg_Card_Payment_Response_DTO>(_paymentRegServiceResponseContent);
                            return new ObjectResult(reg_card_res);
                        }

                    }
                    else if (_paymentRegServiceResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                    {

                        _paymentRegServiceResponseContent = await _paymentRegServiceResponse.Content.ReadAsStringAsync();
                        HttpResponseMessage formatedErrorDetails = await _utilityService.GetFormatedErrors(_paymentRegServiceResponseContent, vendorCode, lobCode);
                        string content = await formatedErrorDetails.Content.ReadAsStringAsync();
                        DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(content);
                        var formatedRes = new ObjectResult(errorDetails);
                        formatedRes.StatusCode = errorDetails.error.status;
                        return formatedRes;
                    }
                    else
                    {
                        throw new VendorException(vendorCode, lobCode, _paymentRegServiceResponse.StatusCode);
                    }
                }
                else
                {
                    _logger.LogInformation("\nVendorConnect Controller: Request Validation Unsuccessfull for Unregistered User.\n");
                    DPCErrorModel errors = JsonConvert.DeserializeObject<DPCErrorModel>(_paymentReqValidationContent);
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
            catch (BrokenCircuitException e)
            {
                throw new CircuitBreakerException(vendorCode, lobCode);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("\nVendorConnect Controller: Error in UnregisteredPaymentRequestToVendorConnect Method.\n");
                _logger.LogError(ex.StackTrace, ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Get a payment request from Payment Microservice and validate the request if validated
        /// then pass this request to Vendor Connect Service to make payment for unregistered user.
        /// Get the response from service method validate the response if validated then pass the response to Payment Microservice.
        /// In case of error and validation failure return the proper error message.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("UnregisteredPaymentRequestToVendorConnect")]
        //[Route("api/v{version:apiVersion}/unregistered/payment")]
        public async Task<IActionResult> UnregisteredPaymentRequestToVendorConnect()
        {
            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];
            try
            {

                string paymentRequestJsonBody = string.Empty;
                var reader = new StreamReader(Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false);
                paymentRequestJsonBody = await reader.ReadToEndAsync();
                if (paymentRequestJsonBody == null)
                {
                    throw new BadRequestException(vendorCode, lobCode);
                }
                var yourObject = System.Text.Json.JsonDocument.Parse(paymentRequestJsonBody);
                _logger.LogInformation("\nVendorConnect Controller:: Start Payment Processing in ACI.\n");
                _paymentReqValidation = await _utilityService.ValidateRequestResponse(paymentRequestJsonBody, _paymentUnregRequestSchemaName, vendorCode, lobCode);
                var requestValidationResponseContent = await _paymentReqValidation.Content.ReadAsStringAsync();
                if (_paymentReqValidation.StatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation("\nVendorConnect Controller: Request Validation Successfull for Unregistered User.\n");

                    //JObject data = JObject.Parse(paymentRequestJsonBody);
                    //Unreg_Payment_Request_DTO requestDto = data.SelectToken("Unregistered_Card").ToObject<Unreg_Payment_Request_DTO>();
                    Unreg_Payment_Request_DTO requestDto = JsonConvert.DeserializeObject<Unreg_Payment_Request_DTO>(paymentRequestJsonBody);

                    _paymentUnregServiceResponse = await _vendorConnectService.UnregisteredPayment(requestDto);



                    if (_paymentUnregServiceResponse.StatusCode == HttpStatusCode.OK)
                    {
                        _logger.LogInformation("\nVendorConnect Controller: Response Fetched Successfull for Unregistered User.\n");
                        _paymentUnregServiceResponseContent = await _paymentUnregServiceResponse.Content.ReadAsStringAsync();

                        if (_paymentUnregServiceResponseContent == null)
                        {
                            throw new NoContentException(vendorCode, lobCode);
                        }
                        JObject aciPaymentResponseJsonBody = JObject.Parse(_paymentUnregServiceResponseContent);

                        bool kindExist = aciPaymentResponseJsonBody.SelectTokens("fundingAccountSummary.kind").Any();
                        if (!kindExist)
                        {
                            throw new NotAcceptableException(vendorCode, lobCode);
                        }
                        string responseKind = aciPaymentResponseJsonBody.SelectToken("fundingAccountSummary.kind").ToString();
                        responseKind = responseKind.ToLower();


                        if (responseKind.Equals("ach"))
                        {
                            _paymentResValidation = await _utilityService.ValidateRequestResponse(_paymentUnregServiceResponseContent, _paymentUnregAchResponseSchemaName, vendorCode, lobCode);

                            if (_paymentResValidation.StatusCode != HttpStatusCode.OK)
                            {
                                _logger.LogInformation("\nVendorConnect Controller: Response Validated Unsuccessfull for Unregistered User {0}.\n", responseKind);
                                var responseValidationResponseContent = await _paymentResValidation.Content.ReadAsStringAsync();
                                DPCErrorModel errors = JsonConvert.DeserializeObject<DPCErrorModel>(responseValidationResponseContent);
                                return StatusCode(422, errors);

                            }
                            _logger.LogInformation("\nVendorConnect Controller: Response Fetched Successfull for Unregistered User {0}.\n", responseKind);
                            unreg_ach_res = JsonConvert.DeserializeObject<Unreg_Ach_Payment_Response_DTO>(_paymentUnregServiceResponseContent);
                            return new ObjectResult(unreg_ach_res);


                        }
                        else
                        {

                            _paymentResValidation = await _utilityService.ValidateRequestResponse(_paymentUnregServiceResponseContent, _paymentUnregCardResponseSchemaName, vendorCode, lobCode);
                            var responseValidationResponseContent = await _paymentResValidation.Content.ReadAsStringAsync();
                            if (_paymentResValidation.StatusCode != HttpStatusCode.OK)
                            {
                                _logger.LogInformation("\nVendorConnect Controller: Response Validated Unsuccessfull for Unregistered User {0}.\n", responseKind);
                                DPCErrorModel errors = JsonConvert.DeserializeObject<DPCErrorModel>(responseValidationResponseContent);
                                return StatusCode(422, errors);
                            }
                            _logger.LogInformation("\nVendorConnect Controller: Response Fetched Successfull for Unregistered User {0}.\n", responseKind);
                            unreg_card_res = JsonConvert.DeserializeObject<Unreg_Card_Payment_Response_DTO>(_paymentUnregServiceResponseContent);
                            return new ObjectResult(unreg_card_res);
                        }

                    }
                    else if (_paymentUnregServiceResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                    {
                        _paymentUnregServiceResponseContent = await _paymentUnregServiceResponse.Content.ReadAsStringAsync();

                        HttpResponseMessage formatedErrorDetails = await _utilityService.GetFormatedErrors(_paymentUnregServiceResponseContent, vendorCode, lobCode);
                        string content = await formatedErrorDetails.Content.ReadAsStringAsync();
                        DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(content);
                        var formatedRes = new ObjectResult(errorDetails);
                        formatedRes.StatusCode = errorDetails.error.status;
                        return formatedRes;
                    }
                    else
                    {
                        throw new VendorException(vendorCode, lobCode, _paymentUnregServiceResponse.StatusCode);
                    }

                }
                else
                {
                    _logger.LogInformation("\nVendorConnect Controller: Request Validation Unsuccessfull for Unregistered User.\n");
                    DPCErrorModel errors = JsonConvert.DeserializeObject<DPCErrorModel>(requestValidationResponseContent);
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
            catch (BrokenCircuitException e)
            {
                throw new CircuitBreakerException(vendorCode, lobCode);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("\nVendorConnect Controller: Error in UnregisteredPaymentRequestToVendorConnect Method.\n");
                _logger.LogError(ex.StackTrace, ex.Message);
                throw ex;
            }
        }

        #endregion

        #region UserProfile Controller

        // User Profile Flow

        [HttpPost]
        [Route("CreateUserProfile")]
        //[Route("api/v{version:apiVersion}/userprofile")]
        public async Task<IActionResult> CreateUserProfile([FromBody] UserProfile_Request_DTO userProfile_Request_DTO)
        {
            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];
            try
            {
                _logger.LogInformation("VendorConnectController:: Create user profile in ACI for the user ");

                // Request validation
                RequestSchemaName = "UserProfile/UserProfileCreate_RequestSchema.json";
                string userProfileRequest = JsonConvert.SerializeObject(userProfile_Request_DTO);
                RequestValidation = await _utilityService.ValidateRequestResponse(userProfileRequest, RequestSchemaName, vendorCode, lobCode);
                RequestValidationContent = await RequestValidation.Content.ReadAsStringAsync();

                if (RequestValidation.StatusCode == HttpStatusCode.OK)
                {
                    AciResponse = await _vendorConnectService.CreateUserProfile(userProfile_Request_DTO);
                    AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                    if (AciResponse.StatusCode == HttpStatusCode.OK)
                    {
                        // Response validation
                        ResponseSchemaName = "UserProfile/UserProfileCreate_ResponseSchema.json";
                        ResponseValidation = await _utilityService.ValidateRequestResponse(AciResponseContent, ResponseSchemaName, vendorCode, lobCode);
                        ResponseValidationContent = await ResponseValidation.Content.ReadAsStringAsync();
                        if (ResponseValidation.StatusCode == HttpStatusCode.OK)
                        {
                            UserProfile_Response_DTO userProfile = JsonConvert.DeserializeObject<UserProfile_Response_DTO>(AciResponseContent);
                            return new ObjectResult(userProfile);
                        }
                        else if (ResponseValidation.StatusCode == HttpStatusCode.UnprocessableEntity)
                        {
                            ValidationErrors = JsonConvert.DeserializeObject<RootError>(ResponseValidationContent);
                            return StatusCode(422, ValidationErrors);
                        }
                        throw new BadRequestException(vendorCode, lobCode);
                    }
                    else if (AciResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                    {

                        AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                        HttpResponseMessage formatedErrorDetails = await _utilityService.GetFormatedErrors(AciResponseContent, vendorCode, lobCode);
                        string content = await formatedErrorDetails.Content.ReadAsStringAsync();
                        DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(content);
                        var formatedRes = new ObjectResult(errorDetails);
                        formatedRes.StatusCode = errorDetails.error.status;
                        return formatedRes;
                    }
                    else
                    {
                        //throw new VendorException(vendorCode, lobCode, _paymentUnregServiceResponse.StatusCode);
                        throw new VendorException(vendorCode, lobCode, AciResponse.StatusCode);
                    }
                }
                else
                {
                    ValidationErrors = JsonConvert.DeserializeObject<RootError>(RequestValidationContent);
                    return StatusCode(422, ValidationErrors);
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
            catch (BrokenCircuitException e)
            {
                throw new CircuitBreakerException(lobCode, vendorCode);
            }
            //catch (Exception e)
            //{
            //    throw new BadRequestException(lobCode, vendorCode);
            //}

        }


        [HttpGet]
        [Route("GetUserProfile/{profileId}")]
        //[Route("api/v{version:apiVersion}/userprofile/{profileId}")]
        public async Task<IActionResult> GetUserProfile(string profileId)
        {
            _logger.LogInformation("VendorConnectController:: Get user profile in ACI for the user ");
            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];
            try
            {
                AciResponse = await _vendorConnectService.GetUserProfile(profileId);
                AciResponseContent = await AciResponse.Content.ReadAsStringAsync();

                if (AciResponse.StatusCode == HttpStatusCode.OK)
                {
                    ResponseSchemaName = "UserProfile/UserProfileCreate_ResponseSchema.json";
                    AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                    // Response Validation
                    ResponseValidation = await _utilityService.ValidateRequestResponse(AciResponseContent, ResponseSchemaName, vendorCode, lobCode);
                    ResponseValidationContent = await ResponseValidation.Content.ReadAsStringAsync();

                    if (ResponseValidation.StatusCode == HttpStatusCode.OK)
                    {
                        UserProfile_Response_DTO userProfile = JsonConvert.DeserializeObject<UserProfile_Response_DTO>(AciResponseContent);
                        return new ObjectResult(userProfile);
                    }
                    else if (ResponseValidation.StatusCode == HttpStatusCode.UnprocessableEntity)
                    {
                        ValidationErrors = JsonConvert.DeserializeObject<RootError>(ResponseValidationContent);
                        return StatusCode(422, ValidationErrors);
                    }
                    throw new BadRequestException(vendorCode, lobCode);

                }
                else if (AciResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                {

                    AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                    HttpResponseMessage formatedErrorDetails = await _utilityService.GetFormatedErrors(AciResponseContent, vendorCode, lobCode);
                    string content = await formatedErrorDetails.Content.ReadAsStringAsync();
                    DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(content);
                    var formatedRes = new ObjectResult(errorDetails);
                    formatedRes.StatusCode = errorDetails.error.status;
                    return formatedRes;
                }
                else
                {
                    throw new VendorException(vendorCode, lobCode, _paymentUnregServiceResponse.StatusCode);
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
            catch (BrokenCircuitException e)
            {
                throw new CircuitBreakerException(lobCode, vendorCode);
            }
            catch (Exception e)
            {
                throw new BadRequestException(lobCode, vendorCode);
            }
        }


        [HttpPatch]
        [Route("UpdateUserProfile/{profileId}")]
        //[Route("api/v{version:apiVersion}/userprofile/{profileId}")]
        public async Task<IActionResult> UpdateUserProfile(string profileId, UserProfile_Request_DTO userProfile_Request_DTO)
        {
            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];
            try
            {
                _logger.LogInformation("VendorConnectController:: Update user profile in ACI for the user ");

                // Request validation
                RequestSchemaName = "UserProfile/UserProfileUpdate_RequestSchema.json";
                string userProfileRequest = JsonConvert.SerializeObject(userProfile_Request_DTO);
                RequestValidation = await _utilityService.ValidateRequestResponse(userProfileRequest, RequestSchemaName, vendorCode, lobCode);
                RequestValidationContent = await RequestValidation.Content.ReadAsStringAsync();

                if (RequestValidation.StatusCode == HttpStatusCode.OK)
                {
                    AciResponse = await _vendorConnectService.UpdateUserProfile(profileId, userProfile_Request_DTO);
                    AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                    if (AciResponse.StatusCode == HttpStatusCode.OK)
                    {
                        //Response validation
                        ResponseSchemaName = "UserProfile/UserProfileCreate_ResponseSchema.json";
                        string userProfileResponse = JsonConvert.SerializeObject(AciResponse);
                        ResponseValidation = await _utilityService.ValidateRequestResponse(AciResponseContent, ResponseSchemaName, vendorCode, lobCode);
                        ResponseValidationContent = await ResponseValidation.Content.ReadAsStringAsync();
                        if (ResponseValidation.StatusCode == HttpStatusCode.OK)
                        {
                            UserProfile_Response_DTO userProfile = JsonConvert.DeserializeObject<UserProfile_Response_DTO>(AciResponseContent);
                            return new ObjectResult(userProfile);
                        }
                        else if (ResponseValidation.StatusCode == HttpStatusCode.UnprocessableEntity)
                        {
                            ValidationErrors = JsonConvert.DeserializeObject<RootError>(ResponseValidationContent);
                            return StatusCode(422, ValidationErrors);
                        }
                        throw new BadRequestException(vendorCode, lobCode);
                    }
                    else if (AciResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                    {

                        AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                        HttpResponseMessage formatedErrorDetails = await _utilityService.GetFormatedErrors(AciResponseContent, vendorCode, lobCode);
                        string content = await formatedErrorDetails.Content.ReadAsStringAsync();
                        DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(content);
                        var formatedRes = new ObjectResult(errorDetails);
                        formatedRes.StatusCode = errorDetails.error.status;
                        return formatedRes;
                    }
                    else
                    {
                        throw new VendorException(vendorCode, lobCode, _paymentUnregServiceResponse.StatusCode);
                    }
                }
                else
                {
                    ValidationErrors = JsonConvert.DeserializeObject<RootError>(RequestValidationContent);
                    return StatusCode(422, ValidationErrors);
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
            catch (BrokenCircuitException e)
            {
                throw new CircuitBreakerException(lobCode, vendorCode);
            }
            catch (Exception e)
            {
                throw new BadRequestException(lobCode, vendorCode);
            }

        }

        #endregion

        #region Validate funding account
        [HttpPost]
        [Route("ValidateFundingAccount")]
        public async Task<IActionResult> ValidateFundingAccount([FromBody] ValidateFA_RequestDTO fundingAccountRequest)
        {
            _logger.LogInformation("VendorConnectController:: Validating funding account in ACI for the user. ");
            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];
            try
            {
                AciResponse = await _vendorConnectService.ValidateFundingAccount(fundingAccountRequest);
                AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                if (AciResponse.StatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation("VendorConnectController::  Funding account validated successfully.");
                    ValidateFA_ResponseDTO fundingAccountResponse = JsonConvert.DeserializeObject<ValidateFA_ResponseDTO>(AciResponseContent);
                    return new ObjectResult(fundingAccountResponse);
                }
                else if (AciResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                {

                    AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                    HttpResponseMessage formatedErrorDetails = await _utilityService.GetFormatedErrors(AciResponseContent, vendorCode, lobCode);
                    string content = await formatedErrorDetails.Content.ReadAsStringAsync();
                    DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(content);
                    var formatedRes = new ObjectResult(errorDetails);
                    formatedRes.StatusCode = errorDetails.error.status;
                    return formatedRes;
                }
                else
                {
                    throw new VendorException(vendorCode, lobCode, _paymentUnregServiceResponse.StatusCode);
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
            catch (BrokenCircuitException e)
            {
                throw new CircuitBreakerException(lobCode, vendorCode);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("\nVendorConnect Controller: Following exception occurred in ValidateFundingAccount \n");
                _logger.LogError(ex.StackTrace, ex.Message);
                throw ex;
            }
        }
        #endregion

        #region Update Communication Preferences

        [HttpPatch]
        //[Route("api/v{version:apiVersion}/profiles/{profile_id}")]
        //[Route("/v{version:apiVersion}/user-profile/communication-preferences/{profile-id}/{kind}")]
        [Route("UpdateCommunicationPreferences/{profile-id}/{kind}")]
        public async Task<IActionResult> UpdateCommunicationPreferences([FromRoute(Name = "profile-id")] string profileId, [FromRoute(Name = "kind")] string kind, CommunicationPreference_Request_DTO comPrefRequestDto)
        {
            const string _commPrefUpdate_ReqSchemaName = "UserProfile/ComPrefUpdate_Schema.json";
            const string _commPrefUpdate_ResSchemaName = "UserProfile/ComPrefUpdate_Schema.json";
            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];
            try
            {
                _logger.LogInformation("UserProfileController::Update communication preferences called for given profile id and kind: ");

                // Request validation
                string comPrefRequest = JsonConvert.SerializeObject(comPrefRequestDto);
                RequestValidation = await _utilityService.ValidateRequestResponse(comPrefRequest, _commPrefUpdate_ReqSchemaName, vendorCode, lobCode);
                RequestValidationContent = await RequestValidation.Content.ReadAsStringAsync();

                if (RequestValidation.StatusCode == HttpStatusCode.OK)
                {
                    AciResponse = await _vendorConnectService.UpdateCommunicationPreferences(profileId, kind, comPrefRequestDto);
                    AciResponseContent = await AciResponse.Content.ReadAsStringAsync();

                    // Successful user profile creation
                    if (AciResponse.StatusCode == HttpStatusCode.OK)
                    {
                        // Response validation
                        ResponseValidation = await _utilityService.ValidateRequestResponse(AciResponseContent, _commPrefUpdate_ResSchemaName, vendorCode, lobCode);
                        ResponseValidationContent = await ResponseValidation.Content.ReadAsStringAsync();

                        if (ResponseValidation.StatusCode == HttpStatusCode.OK)
                        {
                            CommunicationPreference_Response_DTO comPrefResponse = JsonConvert.DeserializeObject<CommunicationPreference_Response_DTO>(AciResponseContent);
                            _logger.LogInformation("UserProfileController::User Profile updated successfully.");
                            return new ObjectResult(comPrefResponse);
                        }
                        else if (ResponseValidation.StatusCode == HttpStatusCode.UnprocessableEntity)
                        {
                            var errors = JsonConvert.DeserializeObject<RootError>(ResponseValidationContent);
                            return StatusCode(422, errors);
                        }
                        else
                        {
                            var errorResponse = new ObjectResult(ResponseValidationContent);
                            errorResponse.StatusCode = (int)ResponseValidation.StatusCode;
                            return errorResponse; ;
                        }
                    }
                    else if (AciResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                    {

                        AciResponseContent = await AciResponse.Content.ReadAsStringAsync();
                        HttpResponseMessage formatedErrorDetails = await _utilityService.GetFormatedErrors(AciResponseContent, vendorCode, lobCode);
                        string content = await formatedErrorDetails.Content.ReadAsStringAsync();
                        DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(content);
                        var formatedRes = new ObjectResult(errorDetails);
                        formatedRes.StatusCode = errorDetails.error.status;
                        return formatedRes;
                    }
                    else
                    {
                        throw new VendorException(vendorCode, lobCode, _paymentUnregServiceResponse.StatusCode);
                    }
                }
                // Request is not valid
                else
                {
                    var errors = JsonConvert.DeserializeObject<RootError>(RequestValidationContent);
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
            catch (BrokenCircuitException e)
            {
                throw new CircuitBreakerException(lobCode, vendorCode);
            }

            catch (Exception ex)
            {
                _logger.LogError("UserProfileController::Following exception occurred in UpdateCommunicationPreferences " + ex.ToString());
                throw;
            }
        }
        #endregion

    }
}

