using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Request;
using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Response.Registered;
using FundingAccount.API.Domain.Entities;
using FundingAccount.API.Domain.Exceptions;
using FundingAccount.API.FundingAccount_Schema.Model.ValidationErrorModels;
using FundingAccount.API.Utility.ReadRequest;
using System.Reflection.Metadata;

namespace FundingAccount.API.Controllers.v1.Registered
{

    [ApiController]
    [ApiVersion("1")]
    public class Registered_FundingAccountController : ControllerBase
    {

        #region Variables
        private readonly IRegisteredFundingAccountService _fundingAccountService;
        private readonly IUtilityService _utilityService;
        private readonly ILogger<Registered_FundingAccountController> _logger;
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
        private const string _fundingAccRegCard_ReqSchemaName = "FundingAccount/FundingAccount_RegCard_RequestSchema.json";
        private const string _fundingAccRegCard_ResSchemaName = "FundingAccount/FundingAccount_RegCard_ResponseSchema.json";
        private const string _fundingAccRegACH_ReqSchemaName = "FundingAccount/FundingAccount_RegACH_RequestSchema.json";
        private const string _fundingAccRegACH_ResSchemaName = "FundingAccount/FundingAccount_RegACH_ResponseSchema.json";

        #endregion

        #endregion


        public Registered_FundingAccountController(IRegisteredFundingAccountService fundingaccountSvc, IUtilityService utilityService, ILogger<Registered_FundingAccountController> logger, IOptionsSnapshot<AppSettings> appSettings)
        {
            _fundingAccountService = fundingaccountSvc;
            _utilityService = utilityService;
            _logger = logger;

            _appSettings = appSettings.Value;

        }

        #region GetAllFundingAccount
        [HttpGet]
        [Route("/v{version:apiVersion}/registered/{profile-id}/funding-accounts")]
        public async Task<IActionResult> GetAllFundingAccount([FromRoute(Name = "profile-id")] string profileId)
        {
            _vendorCode = Request.Headers["Vendor-Code"];
            _lobCode = Request.Headers["Lob-Code"];

            try
            {
                _logger.LogInformation("\nGet All Funding Account Method called for REGISTERED USER.\n");
                _fundingAccServiceResponse = await _fundingAccountService.GetAllFundingAccount(profileId, _vendorCode, _lobCode);
                if(_fundingAccServiceResponse.StatusCode == HttpStatusCode.OK) {

                    _fundingAccServiceResponseContent = await _fundingAccServiceResponse.Content.ReadAsStringAsync();

                    if(_fundingAccServiceResponseContent == null)
                    {
                        throw new NoContentException(_vendorCode, _lobCode);
                    }

                    Reg_GetAllFA_ResponseDTO reg_GetAllFA_ResponseDTO = JsonConvert.DeserializeObject<Reg_GetAllFA_ResponseDTO>(_fundingAccServiceResponseContent);
                    var response = new ObjectResult(reg_GetAllFA_ResponseDTO);
                    response.StatusCode = (int)_fundingAccServiceResponse.StatusCode;
                    return response;

                }
                else if(_fundingAccServiceResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                {
                    _fundingAccServiceResponseContent = await _fundingAccServiceResponse.Content.ReadAsStringAsync();


                    DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(_fundingAccServiceResponseContent);
                    var formatedRes = new ObjectResult(errorDetails);
                    formatedRes.StatusCode = errorDetails.error.status;
                    return formatedRes;
                }
                else
                {
                    throw new BadRequestException(_vendorCode, _lobCode);
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

        #region CreateRegisteredFundingAccount

        /// <summary>
        /// Get the request to create registered funding account, validate it and route it to specific ACH/Card methods based on kind.
        /// </summary>
        /// <returns>
        /// Validate the response received from ACI and send it to DPC UI. 
        /// </returns>

        [HttpPost]
        [Route("/v{version:apiVersion}/registered/profiles/{profile-id}/funding-accounts")]
        public async Task<IActionResult> CreateFundingAccount_RegisteredUser()
        {
            _vendorCode = Request.Headers["Vendor-Code"];
            _lobCode = Request.Headers["Lob-Code"];
            try
            {
                _logger.LogInformation("\nCreate Funding Account Method called for REGISTERED USER.\n");

                // Get header values
                


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
                    string requestKind = data.SelectToken("*.kind").ToString();
                    switch (requestKind)
                    {
                        case "Card":
                            _logger.LogInformation("\nRequest kind is : Card\n");
                            Reg_FA_Card_Request_DTO reg_FA_Card_Request_DTO = data.SelectToken("Registered_Card").ToObject<Reg_FA_Card_Request_DTO>();
                            var reg_FA_Card_Response = await CreateCardFundingAccount_RegisteredUser(reg_FA_Card_Request_DTO, _vendorCode, _lobCode);
                            if (reg_FA_Card_Response.StatusCode == HttpStatusCode.UnprocessableEntity)
                            {
                                var reg_FA_Card_Res_Content = await reg_FA_Card_Response.Content.ReadAsStringAsync();
                                DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(reg_FA_Card_Res_Content);
                                var formatedRes = new ObjectResult(errorDetails);
                                formatedRes.StatusCode = errorDetails.error.status;
                                return formatedRes;
                            }
                            else if(reg_FA_Card_Response.StatusCode != HttpStatusCode.OK)
                            {
                                throw new BadRequestException(_vendorCode, _lobCode);
                            }
                           
                            // Response validation
                            string aciCardResponse = JsonConvert.SerializeObject(reg_FA_Card_Response);
                            _fundingAccResValidation = await _utilityService.ValidateRequestResponse(aciCardResponse, _fundingAccRegCard_ResSchemaName, _vendorCode, _lobCode);
                            _fundingAccResValidationContent = await _fundingAccResValidation.Content.ReadAsStringAsync();

                            if (_fundingAccResValidation.StatusCode == HttpStatusCode.OK)
                            {
                                return new ObjectResult(reg_FA_Card_Response);
                            }
                            else
                            {
                                DPCErrorModel errorResContent = JsonConvert.DeserializeObject<DPCErrorModel>(_fundingAccResValidationContent);
                                var response = new ObjectResult(errorResContent);
                                response.StatusCode = errorResContent.error.status;
                                return response;
                            }

                        case "Ach":
                            _logger.LogInformation("\nRequest kind is : ACH");
                            Reg_FA_ACH_Request_DTO reg_FA_ACH_Request_DTO = data.SelectToken("Registered_ACH").ToObject<Reg_FA_ACH_Request_DTO>();
                            var reg_FA_ACH_Response = await CreateAchFundingAccount_RegisteredUser(reg_FA_ACH_Request_DTO, _vendorCode, _lobCode);

                            // Response validation
                            string aciAchResponse = JsonConvert.SerializeObject(reg_FA_ACH_Response);
                            _fundingAccResValidation = await _utilityService.ValidateRequestResponse(aciAchResponse, _fundingAccRegACH_ResSchemaName, _vendorCode, _lobCode);
                            _fundingAccResValidationContent = await _fundingAccResValidation.Content.ReadAsStringAsync();

                            if (_fundingAccResValidation.StatusCode == HttpStatusCode.OK)
                            {
                                return new ObjectResult(reg_FA_ACH_Response);
                            }
                            else
                            {
                                DPCErrorModel errorResContent = JsonConvert.DeserializeObject<DPCErrorModel>(_fundingAccResValidationContent);
                                var response = new ObjectResult(errorResContent);
                                response.StatusCode = errorResContent.error.status;
                                return response;
                            }
                    }
                }
                // if Request is invalid
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

        #region CreateRegisteredCardFundingAccount
        /// <summary>
        /// Get the request to create Registered Card funding account, forward it to the service class method
        /// </summary>
        /// <returns>
        /// Receive the response from service class method and forward it.
        /// </returns>
        [HttpPost]
        //[Route("api/v{version:apiVersion}/[controller]/billers/active")]
        [Route("CreateCardFundingAccount_RegisteredUser")]
        private async Task<HttpResponseMessage> CreateCardFundingAccount_RegisteredUser(Reg_FA_Card_Request_DTO fundingAccount, string vendorCode, string lobCode)
        {
            try
            {
                _logger.LogInformation("In Registered Create Card Funding Account Controller method");
                _fundingAccServiceResponse = await _fundingAccountService.RegisteredCardFundingAccount<HttpResponseMessage>(fundingAccount, vendorCode, lobCode);
                _fundingAccServiceResponseContent = await _fundingAccServiceResponse.Content.ReadAsStringAsync();
                return _fundingAccServiceResponse;

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

        #region CreateRegisteredACHFundingAccount
        /// <summary>
        /// Get the request to create Registered ACH funding account, forward it to the service class method
        /// </summary>
        /// <returns>
        /// Receive the response from service class method and forward it.
        /// </returns>

        [HttpPost]
        //[Route("api/v{version:apiVersion}/[controller]/billers/active")]
        [Route("unregistereduser/Achfundingaccount")]
        private async Task<HttpResponseMessage> CreateAchFundingAccount_RegisteredUser(Reg_FA_ACH_Request_DTO fundingAccount, string vendorCode, string lobCode)
        {
            try
            {
                _logger.LogInformation("In Unregistered Create Card Funding Account Controller method");
                _fundingAccServiceResponse = await _fundingAccountService.RegisteredACHFundingAccount<HttpResponseMessage>(fundingAccount, vendorCode, lobCode);
                _fundingAccServiceResponseContent = await _fundingAccServiceResponse.Content.ReadAsStringAsync();

                return _fundingAccServiceResponse;

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

        #region DeleteFundigAccount_RegisteredUser
        /// <summary>
        /// Get the request to delete Registered funding account, forward it to the service class method
        /// </summary>
        /// <returns>
        /// Receive the response from service class method and forward it.
        /// </returns>
        /// 
        [HttpDelete]
        [Route("/v{version:apiVersion}/registered/profiles/{profile-id}/funding-accounts/{funding-account-id}")]
        public async Task<IActionResult> DeleteFundingAccount_RegisteredUser([FromRoute(Name = "profile-id")] string profileId, [FromRoute(Name = "funding-account-id")] string fundingAccountId)
        {
            try
            {
                _logger.LogInformation("Deleting funding account for Registered payer");

                // Get header values
                _vendorCode = Request.Headers["Vendor-Code"];
                _lobCode = Request.Headers["Lob-Code"];

                _fundingAccServiceResponse = await _fundingAccountService.DeleteFundingAccount_RegisteredUser<HttpResponseMessage>(profileId, fundingAccountId, _vendorCode, _lobCode);
                _fundingAccServiceResponseContent = await _fundingAccServiceResponse.Content.ReadAsStringAsync();

                if (_fundingAccServiceResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    _logger.LogInformation("FundingAccountController::Registered Funding account deleted successfully");
                    ObjectResult deleteResponse = new ObjectResult(_fundingAccServiceResponseContent);
                    deleteResponse.StatusCode = (int)_fundingAccServiceResponse.StatusCode;
                    return deleteResponse;
                }
                else
                {
                    throw new NotFoundException(_vendorCode,_lobCode);
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
