using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Request;
using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Request.Unregistered;
using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Response;
using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Response.Unregistered;
using FundingAccount.API.Domain.Entities;
using FundingAccount.API.Domain.Exceptions;
using FundingAccount.API.FundingAccount_Schema.Model.ValidationErrorModels;
using FundingAccount.API.Utility.ReadRequest;
using System.Reflection.Metadata;

namespace FundingAccount.API.Controllers.v1.Common
{
    [ApiController]
    [ApiVersion("1")]
    public class FundingAccountController : ControllerBase
    {
        #region Variables
        private readonly ICommonFundingAccountService _fundingAccountService;
        private readonly IUtilityService _utilityService;
        private readonly ILogger<FundingAccountController> _logger;
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
        private string _fundingAccReqSchemaName = "FundingAccount_RegUnreg_RequestSchema.json";
        private const string _fundingAccUnregCard_ReqSchemaName = "FundingAccount_UnregCard_RequestSchema.json";
        private const string _fundingAccUnregCard_ResSchemaName = "FundingAccount_UnregCard_ResponseSchema.json";
        private const string _fundingAccUnregACH_ReqSchemaName = "FundingAccount_UnregACH_RequestSchema.json";
        private const string _fundingAccUnregACH_ResSchemaName = "FundingAccount_UnregACH_ResponseSchema.json";
        #endregion

        #endregion

        public FundingAccountController(ICommonFundingAccountService fundingaccountSvc, IUtilityService utilityService, ILogger<FundingAccountController> logger, IOptionsSnapshot<AppSettings> appSettings)
        {
            _fundingAccountService = fundingaccountSvc;
            _utilityService = utilityService;
            _logger = logger;

            _appSettings = appSettings.Value;
            //_schemaName = _appSettings.SchemaName;
        }



        #region ValidateFundingAccount
        /// <summary>
        /// Get the request to validate funding account, forward it to the service class method
        /// </summary>
        /// <returns>
        /// Receive the response from service class method and forward it.
        /// </returns>
        [HttpPost]
        [Route("/v{version:apiVersion}/funding-accounts/validate")]
        public async Task<IActionResult> ValidateFundingAccount()
        {
            _vendorCode = Request.Headers["Vendor-Code"];
            _lobCode = Request.Headers["Lob-Code"];

            try
            {
                _logger.LogInformation("In validate Funding Account Controller method");
                // Get header values
               
                // Extract body 
                string fundingAccountJsonBody = string.Empty;
                fundingAccountJsonBody = await ReadRequestBody.GetRequestRawBodyAsync(Request, _logger, Encoding.UTF8);
                JObject data = JObject.Parse(fundingAccountJsonBody);
                FA_Validate_Request_DTO fundingAccountRequest = JsonConvert.DeserializeObject<FA_Validate_Request_DTO>(fundingAccountJsonBody);
                _fundingAccServiceResponse = await _fundingAccountService.ValidateFundingAccount<HttpResponseMessage>(fundingAccountRequest, _vendorCode, _lobCode);
                _fundingAccServiceResponseContent = await _fundingAccServiceResponse.Content.ReadAsStringAsync();

                if (_fundingAccServiceResponse.StatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation("FundingAccountController::Registered Card Funding account fetched successfully");
                    FA_Validate_Response_DTO fundingAccountResponse = JsonConvert.DeserializeObject<FA_Validate_Response_DTO>(_fundingAccServiceResponseContent);

                    return new ObjectResult(fundingAccountResponse);
                }
                else if (_fundingAccServiceResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                {

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
    }


}
