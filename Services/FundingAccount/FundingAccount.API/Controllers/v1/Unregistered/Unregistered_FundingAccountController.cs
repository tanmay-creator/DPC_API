using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount;
using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Request.Unregistered;
using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Response.Unregistered;
using FundingAccount.API.Domain.Entities;
using FundingAccount.API.Domain.Exceptions;
using FundingAccount.API.FundingAccount_Schema.Model.ValidationErrorModels;
using FundingAccount.API.Utility.ReadRequest;
using System.Text;

namespace FundingAccount.API.Controllers.v1.Unregistered
{
    //[Route("api/v{version:apiVersion}/[controller]")]
    //[Route("[controller]")]
    [ApiController]
    [ApiVersion("1")]
    //[ApiVersion("2.0")]
    public class Unregistered_FundingAccountController : ControllerBase
    {
        #region Variables
        private readonly IUnregisteredFundingAccountService _fundingAccountService;
        private readonly IUtilityService _utilityService;
        private readonly ILogger<Unregistered_FundingAccountController> _logger;
        private AppSettings _appSettings { get; }

        private HttpResponseMessage _fundingAccReqValidation;
        private HttpResponseMessage _fundingAccResValidation;
        private HttpResponseMessage _fundingAccServiceResponse;
        private string _fundingAccReqValidationContent;
        private string _fundingAccResValidationContent;
        private string _fundingAccServiceResponseContent;
        private string _vendorCode;
        private string _lobCode;


        #region Schema File Names
        private string _fundingAccReqSchemaName = "FundingAccount/FundingAccount_RegUnreg_RequestSchema.json";
        private const string _fundingAccUnregCard_ReqSchemaName = "FundingAccount/FundingAccount_UnregCard_RequestSchema.json";
        private const string _fundingAccUnregCard_ResSchemaName = "FundingAccount/FundingAccount_UnregCard_ResponseSchema.json";
        private const string _fundingAccUnregACH_ReqSchemaName = "FundingAccount/FundingAccount_UnregACH_RequestSchema.json";
        private const string _fundingAccUnregACH_ResSchemaName = "FundingAccount/FundingAccount_UnregACH_ResponseSchema.json";
        #endregion

        #endregion

        public Unregistered_FundingAccountController(IUnregisteredFundingAccountService fundingaccountSvc, IUtilityService utilityService, ILogger<Unregistered_FundingAccountController> logger, IOptionsSnapshot<AppSettings> appSettings)
        {
            _fundingAccountService = fundingaccountSvc;
            _utilityService = utilityService;
            _logger = logger;

            _appSettings = appSettings.Value;
            //_schemaName = _appSettings.SchemaName;
        }

        #region CreateUnregisteredFundingAccount

        /// <summary>
        /// Get the request to create unregistered funding account, validate it and route it to specific ACH/Card methods based on kind.
        /// </summary>
        /// <returns>
        /// Validate the response received from ACI and send it to DPC UI. 
        /// </returns>

        [HttpPost]
        [Route("/v{version:apiVersion}/unregistered/funding-accounts")]
        public async Task<IActionResult> CreateFundingAccount_UnregisteredUser()
        {
            try
            {
                _logger.LogInformation("\nCreate Funding Account Method called for UNREGISTERED USER.\n");

                // Get header values
                _vendorCode = Request.Headers["Vendor-Code"];
                _lobCode = Request.Headers["Lob-Code"];

                // Extract body 
                string fundingAccountJsonBody = string.Empty;
                fundingAccountJsonBody = await ReadRequestBody.GetRequestRawBodyAsync(Request, _logger, Encoding.UTF8);
                JObject data = JObject.Parse(fundingAccountJsonBody);

                // Request Validation
                _fundingAccReqValidation = await _utilityService.ValidateRequestResponse(fundingAccountJsonBody, _fundingAccReqSchemaName, _vendorCode, _lobCode);
                _fundingAccReqValidationContent = await _fundingAccReqValidation.Content.ReadAsStringAsync();

                if (_fundingAccReqValidation.StatusCode == HttpStatusCode.OK)
                {
                    //string requestKind = data.SelectToken("Unregistered_ACH.kind").ToString();
                    
                    bool kindExist = data.SelectTokens("*.kind").Any();
                    if (!kindExist)
                    {
                        throw new NotAcceptableException(_vendorCode, _lobCode);
                    }
                    string requestKind = data.SelectToken("*.kind").ToString();
                    requestKind = requestKind.ToLower();
                    switch (requestKind)
                    {
                        case "card":
                            _logger.LogInformation("\nRequest kind is : \n" + requestKind);
                            Unreg_FA_Card_Request_DTO unreg_FA_Card_Request_DTO = data.SelectToken("Unregistered_Card").ToObject<Unreg_FA_Card_Request_DTO>();
                            HttpResponseMessage unreg_FA_Card_Response = await CreateCardFundingAccount_UnregisteredUser(unreg_FA_Card_Request_DTO, _vendorCode, _lobCode);
                            string unreg_FA_CardResponseContent = await unreg_FA_Card_Response.Content.ReadAsStringAsync();
                            if (unreg_FA_Card_Response.StatusCode == HttpStatusCode.UnprocessableEntity)
                            {

                                DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(unreg_FA_CardResponseContent);
                                var formatedRes = new ObjectResult(errorDetails);
                                formatedRes.StatusCode = errorDetails.error.status;
                                return formatedRes;
                            }
                            else if (unreg_FA_CardResponseContent == null)
                            {
                                throw new NotFoundException(_vendorCode,_lobCode);
                            }
                            // Response validation
                            else if (unreg_FA_Card_Response.IsSuccessStatusCode)
                            {
                                _fundingAccResValidation = await _utilityService.ValidateRequestResponse(unreg_FA_CardResponseContent, _fundingAccUnregCard_ResSchemaName, _vendorCode, _lobCode);
                                _fundingAccResValidationContent = await _fundingAccResValidation.Content.ReadAsStringAsync();
                                Unreg_FA_Card_Response_DTO CardResponseDto = JsonConvert.DeserializeObject<Unreg_FA_Card_Response_DTO>(unreg_FA_CardResponseContent);

                                if (_fundingAccResValidation.StatusCode == HttpStatusCode.OK)
                                {
                                    return new ObjectResult(CardResponseDto);
                                }
                                else
                                {
                                    DPCErrorModel errorResContent = JsonConvert.DeserializeObject<DPCErrorModel>(_fundingAccResValidationContent);
                                    var errorCardResponse = new ObjectResult(errorResContent);
                                    errorCardResponse.StatusCode = errorResContent.error.status;
                                    return errorCardResponse;
                                }
                            }
                            else
                            {
                                throw new BadRequestException(_vendorCode, _lobCode);
                            }


                        case "ach":
                            _logger.LogInformation("\nRequest kind is : " + requestKind);
                            Unreg_FA_ACH_Request_DTO unreg_FA_ACH_Request = data.SelectToken("Unregistered_ACH").ToObject<Unreg_FA_ACH_Request_DTO>();
                            HttpResponseMessage unreg_FA_ACH_Response = await CreateAchFundingAccount_UnregisteredUser(unreg_FA_ACH_Request, _vendorCode, _lobCode);
                            string unreg_FA_ACH_ResponseContent = await unreg_FA_ACH_Response.Content.ReadAsStringAsync();
                            if (unreg_FA_ACH_Response.StatusCode == HttpStatusCode.UnprocessableEntity)
                            {

                                DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(unreg_FA_ACH_ResponseContent);
                                var formatedRes = new ObjectResult(errorDetails);
                                formatedRes.StatusCode = errorDetails.error.status;
                                return formatedRes;
                            }
                            else if (unreg_FA_ACH_ResponseContent == null)
                            {
                                throw new NotFoundException(_vendorCode, _lobCode);
                            }
                            // Response validation
                            else if (unreg_FA_ACH_Response.IsSuccessStatusCode)
                            {
                                _fundingAccResValidation = await _utilityService.ValidateRequestResponse(unreg_FA_ACH_ResponseContent, _fundingAccUnregACH_ResSchemaName, _vendorCode, _lobCode);
                                _fundingAccResValidationContent = await _fundingAccResValidation.Content.ReadAsStringAsync();
                                Unreg_FA_ACH_Response_DTO ACHResponseDto = JsonConvert.DeserializeObject<Unreg_FA_ACH_Response_DTO>(unreg_FA_ACH_ResponseContent);
                                if (_fundingAccResValidation.StatusCode == HttpStatusCode.OK)
                                {
                                    return new ObjectResult(ACHResponseDto);
                                }
                                else
                                {
                                    DPCErrorModel errorResContent = JsonConvert.DeserializeObject<DPCErrorModel>(_fundingAccResValidationContent);
                                    var errorCardResponse = new ObjectResult(errorResContent);
                                    errorCardResponse.StatusCode = errorResContent.error.status;
                                    return errorCardResponse;
                                }
                            }
                            else
                            {
                                throw new BadRequestException(_vendorCode, _lobCode);
                            }
                    }
                }
                // If Request is invalid

                DPCErrorModel errorContent = JsonConvert.DeserializeObject<DPCErrorModel>(_fundingAccReqValidationContent);
                var errorResponse = new ObjectResult(errorContent);
                errorResponse.StatusCode = errorContent.error.status;
                return errorResponse;
            }
            catch (HttpRequestException ex)
            {
                throw new ServiceNotAvailableException(_vendorCode, _lobCode);
            }
            catch (JsonReaderException ex)
            {
                throw new BadRequestException(_vendorCode, _lobCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #region CreateUnregisteredCardFundingAccount

        /// <summary>
        /// Get the request to create Unregistered Card funding account, forward it to the service class method
        /// </summary>
        /// <returns>
        /// Receive the response from service class method and forward it.
        /// </returns>
        [HttpPost]
        //[Route("api/v{version:apiVersion}/[controller]/billers/active")]
        [Route("CreateCardFundingAccount_UnregisteredUser")]
        private async Task<HttpResponseMessage> CreateCardFundingAccount_UnregisteredUser(Unreg_FA_Card_Request_DTO fundingAccount, string vendorCode, string lobCode)
        {
            try
            {
                _logger.LogInformation("In Unregistered Create Card Funding Account Controller method");
                _fundingAccServiceResponse = await _fundingAccountService.UnregisteredCardFundingAccount<HttpResponseMessage>(fundingAccount, vendorCode, lobCode);
                _fundingAccServiceResponseContent = await _fundingAccServiceResponse.Content.ReadAsStringAsync();

                return _fundingAccServiceResponse;
                //if( _fundingAccServiceResponseContent == null)
                //{
                //    throw new NotFoundException(vendorCode, lobCode);
                //}
                //if (_fundingAccServiceResponse.StatusCode == HttpStatusCode.OK)
                //{
                //    _logger.LogInformation("FundingAccountController::Unregistered Card Funding account fetched successfully");
                //    //Unreg_FA_Card_Response_DTO unregFACardResponse = JsonConvert.DeserializeObject<Unreg_FA_Card_Response_DTO>(_fundingAccServiceResponseContent);
                //    //return unregFACardResponse;       --object
                //    return _fundingAccServiceResponse;      //--httpresponsemessage

                //}
                //else
                //{
                //    var errorResponse = new HttpResponseMessage();
                //    errorResponse.StatusCode = _fundingAccServiceResponse.StatusCode;
                //    errorResponse.ReasonPhrase = _fundingAccServiceResponse.ReasonPhrase;
                //    errorResponse.Content = _fundingAccServiceResponse.Content;
                //    return errorResponse;
                //}
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

        #region CreateUnregisteredACHFundingAccount
        /// <summary>
        /// Get the request to create Unregistered ACH funding account, forward it to the service class method
        /// </summary>
        /// <returns>
        /// Receive the response from service class method and forward it.
        /// </returns>

        [HttpPost]
        //[Route("api/v{version:apiVersion}/[controller]/billers/active")]
        [Route("unregistereduser/Achfundingaccount")]
        private async Task<HttpResponseMessage> CreateAchFundingAccount_UnregisteredUser(Unreg_FA_ACH_Request_DTO fundingAccount, string vendorCode, string lobCode)
        {
            try
            {
                _logger.LogInformation("In Unregistered Create Card Funding Account Controller method");
                _fundingAccServiceResponse = await _fundingAccountService.UnregisteredACHFundingAccount<HttpResponseMessage>(fundingAccount, vendorCode, lobCode);
                _fundingAccServiceResponseContent = await _fundingAccServiceResponse.Content.ReadAsStringAsync();
                return _fundingAccServiceResponse;
                //if(_fundingAccServiceResponseContent == null)
                //{
                //    throw new NotFoundException(vendorCode, lobCode);
                //}
                //if (_fundingAccServiceResponse.StatusCode == HttpStatusCode.OK)
                //{
                //    _logger.LogInformation("FundingAccountController::Unregistered ACH Funding account fetched successfully");
                //    //Unreg_FA_ACH_Response_DTO unregFAACHResponse = JsonConvert.DeserializeObject<Unreg_FA_ACH_Response_DTO>(_fundingAccServiceResponseContent);
                //    return _fundingAccServiceResponse;
                //}
                //else
                //{
                //    var errorResponse = new HttpResponseMessage();
                //    errorResponse.StatusCode = _fundingAccServiceResponse.StatusCode;
                //    errorResponse.ReasonPhrase = _fundingAccServiceResponse.ReasonPhrase;
                //    errorResponse.Content = _fundingAccServiceResponse.Content;
                //    return errorResponse;
                //}
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

        #region DeleteFundigAccount
        /// <summary>
        /// Get the request to delete Unregistered funding account, forward it to the service class method
        /// </summary>
        /// <returns>
        /// Receive the response from service class method and forward it.
        /// </returns>

        [HttpDelete]
        [Route("/v{version:apiVersion}/unregistered/funding-accounts/{biller-id}/{biller-account-id}/{funding-account-id}")]
        public async Task<IActionResult> DeleteFundingAccount_UnregisteredUser([FromRoute(Name = "biller-id")] string billerId, [FromRoute(Name = "biller-account-id")] string billerAccountId, [FromRoute(Name = "funding-account-id")] string fundingAccountId)
        {
            // Get header values
            _vendorCode = Request.Headers["Vendor-Code"];
            _lobCode = Request.Headers["Lob-Code"];
            try
            {
                _logger.LogInformation("Deleting funding account for unregistered payer");
                _fundingAccServiceResponse = await _fundingAccountService.DeleteFundingAccount_UnregisteredUser<HttpResponseMessage>(billerId, billerAccountId, fundingAccountId, _vendorCode, _lobCode);
                _fundingAccServiceResponseContent = await _fundingAccServiceResponse.Content.ReadAsStringAsync();
                //if(String.IsNullOrEmpty(_fundingAccServiceResponseContent))
                //{
                //    throw new NotFoundException(_vendorCode, _lobCode);
                //}
                if (_fundingAccServiceResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    _logger.LogInformation("FundingAccountController::Unregistered Funding account deleted successfully");
                    ObjectResult deleteResponse = new ObjectResult(_fundingAccServiceResponseContent);
                    deleteResponse.StatusCode = (int)_fundingAccServiceResponse.StatusCode;
                    return deleteResponse;
                }
                else
                {
                    throw new NotFoundException(_vendorCode, _lobCode);
                }


            }
            catch (HttpRequestException ex)
            {
                throw new ServiceNotAvailableException(_vendorCode, _lobCode);
            }
            catch (JsonReaderException ex)
            {
                throw new BadRequestException(_vendorCode, _lobCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion


    }


}
