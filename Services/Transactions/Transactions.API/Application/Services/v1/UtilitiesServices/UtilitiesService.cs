using Newtonsoft.Json;
using Transaction.API.Application.Services.v1.Services.Abstraction;
using Transaction.API.Domain.Exceptions;
using static Transaction.API.Application.Services.APIEndpoint;

namespace Transaction.API.Application.Services.v1.UtilitiesServices
{
    internal sealed class UtilitiesService : IUtilitiesService
    {
        #region Variables
        private readonly HttpClient _apiClient;
        private readonly ILogger<UtilitiesService> _logger;
        private readonly string _utilityServiceBaseUrl;
        public AppSettings _appSettings { get; }
        private HttpResponseMessage _validationResponse;
        #endregion


        public UtilitiesService(HttpClient httpClient, ILogger<UtilitiesService> logger, IOptionsSnapshot<AppSettings> appSettings)
        {
            _apiClient = httpClient;
            _logger = logger;
            _appSettings = appSettings.Value;
            _utilityServiceBaseUrl = _appSettings.UtilityServiceBaseUrl;
        }

        #region Validate Request Response

        public async Task<HttpResponseMessage> ValidateInboundRequest(string jsonRequest, string schemaName, string vendorCode, string lobCode)
        {

            _logger.LogInformation("UtilityService:: Calling utility api for inbound request validation");
            var utilityUrl = UtilityEndPoint.ValidateInboundRequestInUtility(_utilityServiceBaseUrl, schemaName, vendorCode,lobCode);
            if (utilityUrl == null)
            {

                throw new BadRequestException(vendorCode, lobCode);
            }
            HttpContent content = new StringContent(JsonConvert.SerializeObject(jsonRequest), Encoding.UTF8, "application/json");
            // Adding headers
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Add("Vendor-Code", vendorCode);
            _apiClient.DefaultRequestHeaders.Add("Lob-Code", lobCode);

            _validationResponse = await _apiClient.PostAsync(utilityUrl, content);
            return _validationResponse;
        }

        public HttpResponseMessage GetApiErrorDetails(string errorCategory, string vendorCode, string lobCode)
        {

            _logger.LogInformation("UtilityService:: Calling utility api for error Details");
            var utilityUrl = UtilityEndPoint.GetApiErrorDetails(_utilityServiceBaseUrl, errorCategory, vendorCode, lobCode);
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
