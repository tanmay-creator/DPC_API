
using FundingAccount.API.Domain.Exceptions;

namespace FundingAccount.API.Application.Services.v1.UtilitiesServices
{

    internal sealed class UtilitiesService : IUtilityService
    {
        #region Variable
        private readonly HttpClient _apiClient;
        private readonly ILogger<UtilitiesService> _logger;
        private readonly string _utilityServiceBaseUrl;
        private AppSettings _appSettings { get; }
        private readonly string _applicationJson = "application/json";
        private HttpContent Content;
        private HttpResponseMessage _utilityResponse;
        #endregion


        public UtilitiesService(HttpClient httpClient, ILogger<UtilitiesService> logger, IOptionsSnapshot<AppSettings> appSettings)
        {
            _apiClient = httpClient;
            _logger = logger;
            _appSettings = appSettings.Value;
            _utilityServiceBaseUrl = _appSettings.UtilityServiceBaseUrl;
        }

        #region Validation Request Response
        public async Task<HttpResponseMessage> ValidateRequestResponse(string jsonRequest, string schemaName, string vendorCode, string lobCode)
        {
           
                _logger.LogInformation("UtilityService:: Calling utility api for validation");
                var utilityUrl = APIEndpoints.ValidateRequestResponse(_utilityServiceBaseUrl, schemaName, vendorCode, lobCode);
                if(utilityUrl == null)
                {
                    throw new BadRequestException(vendorCode, lobCode);
                }
                Content = new StringContent(JsonConvert.SerializeObject(jsonRequest), Encoding.UTF8, _applicationJson);
                _utilityResponse = await _apiClient.PostAsync(utilityUrl, Content);
                return _utilityResponse;
           
        }
        #endregion

        #region Get API Errors
        public HttpResponseMessage GetApiErrorDetails(string errorCategory, string vendorCode, string lobCode)
        {

            _logger.LogInformation("UtilityService:: Calling utility api for error Details");
            var utilityUrl = APIEndpoints.GetApiErrorDetails(_utilityServiceBaseUrl, errorCategory, vendorCode, lobCode);
            if (utilityUrl == null)
            {

                throw new BadRequestException(vendorCode, lobCode);
            }
            using (var request = new HttpRequestMessage(HttpMethod.Get, utilityUrl))
            {
                var validationResponse = _apiClient.Send(request);
                validationResponse.EnsureSuccessStatusCode();
                return validationResponse;
            }

        }
        #endregion
    }
}
