using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using UserProfile.API.Application.Services.v1.Services.Abstraction;
using UserProfile.API.Domain.Exceptions;
using static UserProfile.API.Application.Services.v1.APIEndpoint;

namespace UserProfile.API.Application.Services.v1.UtilitiesService
{

    internal sealed class UtilitiesService : IUtilityService
    {
        #region Variables
        private readonly HttpClient _apiClient;
        private readonly ILogger<UtilitiesService> _logger;
        private readonly string _utilityServiceBaseUrl;
        private HttpResponseMessage validationResponse;
        public AppSettings _appSettings { get; }
        #endregion


        public UtilitiesService(HttpClient httpClient, ILogger<UtilitiesService> logger, IOptionsSnapshot<AppSettings> appSettings)
        {
            _apiClient = httpClient;
            _logger = logger;
            _appSettings = appSettings.Value;
            _utilityServiceBaseUrl = _appSettings.UtilityServiceBaseUrl;
        }

        #region Validate Request Response
        public async Task<HttpResponseMessage> ValidateRequestResponse(string jsonRequest, string schemaName, string vendorCode, string lobCode)
        {
            try
            {
                _logger.LogInformation("UtilityService:: Calling utility api for  validation");
                var utilityUrl = UserProfileEndpoint.ValidateRequestResponse(_utilityServiceBaseUrl, schemaName, vendorCode, lobCode);
                HttpContent content = new StringContent(JsonConvert.SerializeObject(jsonRequest), Encoding.UTF8, "application/json");
                validationResponse = await _apiClient.PostAsync(utilityUrl, content);
                return validationResponse;

            }
            catch (Exception ex)
            {
                _logger.LogError("Utility Service::Following exception occurred in ValidateRequestResponse \n" + ex.ToString());
                throw;

            }

        }
        #endregion

        public HttpResponseMessage GetApiErrorDetails(string errorCategory, string vendorCode, string lobCode)
        {

            _logger.LogInformation("UtilityService:: Calling utility api for error Details");
            var utilityUrl = UserProfileEndpoint.GetApiErrorDetails(_utilityServiceBaseUrl, errorCategory, vendorCode, lobCode);
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
    }
}
