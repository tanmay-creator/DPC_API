using VendorConnect.API;
using VendorConnect.API.Application.Services.Services.Abstraction;
using VendorConnect.API.Domain.Exceptions;
using static VendorConnect.API.Application.Services.APIEndpoint;

namespace FundingAccount.API.Application.Services
{

    internal sealed class UtilityService : IUtilityService
    {
        #region Variable

        private readonly HttpClient _apiClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UtilityService> _logger;
        private readonly string _utilityServiceBaseUrl;
        private HttpResponseMessage _validationResponse;
        private AppSettings _appSettings { get; }
        #endregion


        public UtilityService(HttpClient httpClient, ILogger<UtilityService> logger, IOptionsSnapshot<AppSettings> appSettings)
        {
            _apiClient = httpClient;
            _logger = logger;
            _appSettings = appSettings.Value;
            _utilityServiceBaseUrl = _appSettings.UtilityServiceBaseUrl;
        }

        #region Validate Request Response
        public async Task<HttpResponseMessage> ValidateRequestResponse(string jsonRequest, string schemaName,string vendorCode, string lobCode)
        {

            _logger.LogInformation("UtilityService:: Calling utility api for inbound request validation");
            var utilityUrl = UtilitiesEndpoint.ValidateRequestResponse(_utilityServiceBaseUrl, schemaName, vendorCode, lobCode);
            if (utilityUrl == null)
            {
                throw new BadRequestException("Aci", "Met_sfd");
            }
            HttpContent content = new StringContent(JsonConvert.SerializeObject(jsonRequest), Encoding.UTF8, "application/json");
            _validationResponse = await _apiClient.PostAsync(utilityUrl, content);
            return _validationResponse;



        }

        #endregion

        public async Task<HttpResponseMessage> GetApiErrorDetails(string errorCategory, string vendorCode, string lobCode)
        {

            _logger.LogInformation("UtilityService:: Calling utility api for error Details");
            var utilityUrl = UtilitiesEndpoint.GetApiErrorDetails(_utilityServiceBaseUrl, errorCategory, vendorCode, lobCode);
            if (utilityUrl == null)
            {

                throw new BadRequestException(vendorCode,lobCode);
            }
            //HttpContent content = new StringContent(JsonConvert.SerializeObject(jsonRequest), Encoding.UTF8, "application/json");
            using (var request = new HttpRequestMessage(HttpMethod.Get, utilityUrl))
            {
                var validationResponse = _apiClient.Send(request);
                validationResponse.EnsureSuccessStatusCode();
                return validationResponse;
            }

        }

        public async Task<HttpResponseMessage> GetFormatedErrors(string jsonRequest,string vendorCode, string lobCode)
        {

            _logger.LogInformation("UtilityService:: Calling utility api for formated Aci error Details");
            var utilityUrl = UtilitiesEndpoint.GetAciValidationInDpcFormat(_utilityServiceBaseUrl, vendorCode, lobCode);
            if (utilityUrl == null)
            {

                throw new BadRequestException(vendorCode, lobCode);
            }
            //HttpContent content = new StringContent(JsonConvert.SerializeObject(jsonRequest), Encoding.UTF8, "application/json");
          
            if (utilityUrl == null)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }
            HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            _validationResponse = await _apiClient.PostAsync(utilityUrl, content);
            return _validationResponse;

        }
    }
}