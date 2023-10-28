using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using UserProfile.API.Application.ModelDTOs.v1.UserProfile.Request;
using UserProfile.API.Application.Services.v1.Services.Abstraction;
using static UserProfile.API.Application.Services.v1.APIEndpoint;

namespace UserProfile.API.Application.Services.v1.UserProfileService
{
    internal sealed class UserProfileService : IUserProfileService
    {
        #region Variables

        private readonly HttpClient _apiClient;
        private readonly ILogger<UserProfileService> _logger;
        private readonly string _vendorConnectBaseUrl;
        private HttpResponseMessage _vendorConnectResponse;
        private string vendorConnectResponseContent;
        private HttpContent content;
        private readonly string _applicationJson = "application/json";
        private AppSettings _appSettings { get; }

        #endregion

        public UserProfileService(HttpClient httpClient, IOptionsSnapshot<AppSettings> appSettings, ILogger<UserProfileService> logger)
        {
            _apiClient = httpClient;
            _appSettings = appSettings.Value;
            _logger = logger;
            _vendorConnectBaseUrl = _appSettings.VendorConnectBaseUrl;
        }

        #region Create User Profile

        public async Task<HttpResponseMessage> CreateUserProfile<T>(UserProfile_Request_DTO userProfile, string vendorCode, string lobCode)
        {
            
                _logger.LogInformation("UserProfileService:: Creating UserProfile for the request. ");
                string vendorConnectCreateUserProfileUrl = UserProfileEndpoint.CreateUserProfile(_vendorConnectBaseUrl);
                content = new StringContent(JsonConvert.SerializeObject(userProfile), Encoding.UTF8, _applicationJson);
                // Adding headers
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Add("Vendor-Code", vendorCode);
                _apiClient.DefaultRequestHeaders.Add("Lob-Code", lobCode);
                _vendorConnectResponse = await _apiClient.PostAsync(vendorConnectCreateUserProfileUrl, content);
                vendorConnectResponseContent = await _vendorConnectResponse.Content.ReadAsStringAsync();
                return _vendorConnectResponse;
           
        }

        #endregion

        #region Get User Profile

        public async Task<HttpResponseMessage> GetUserProfile<T>(string profileId, string vendorCode, string lobCode)
        {
            
                string vendorConnectGetUserProfileUrl = UserProfileEndpoint.GetUserProfile(_vendorConnectBaseUrl, profileId);
                _logger.LogDebug("[GetUserProfile] -> Calling {Uri} to get the user profile", vendorConnectGetUserProfileUrl);
                // Adding headers
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Add("Vendor-Code", vendorCode);
                _apiClient.DefaultRequestHeaders.Add("Lob-Code", lobCode);
                _vendorConnectResponse = await _apiClient.GetAsync(vendorConnectGetUserProfileUrl);
                vendorConnectResponseContent = await _vendorConnectResponse.Content.ReadAsStringAsync();
                return _vendorConnectResponse;
           
        }

        #endregion

        #region Update User Profile

        public async Task<HttpResponseMessage> UpdateUserProfile<T>(string profileId, UserProfile_Request_DTO userProfile_UpdateRequest_DTO, string vendorCode, string lobCode)
        {
            try
            {
                string vendorConnectUpdateUserProfileUrl = UserProfileEndpoint.UpdateUserProfile(_vendorConnectBaseUrl, profileId);
                _logger.LogDebug("[UpdateUserProfile] -> Calling {Uri} to update the user profile", vendorConnectUpdateUserProfileUrl);
                content = new StringContent(JsonConvert.SerializeObject(userProfile_UpdateRequest_DTO), Encoding.UTF8, _applicationJson);
                // Adding headers
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Add("Vendor-Code", vendorCode);
                _apiClient.DefaultRequestHeaders.Add("Lob-Code", lobCode);
                _vendorConnectResponse = await _apiClient.PatchAsync(vendorConnectUpdateUserProfileUrl, content);
                vendorConnectResponseContent = await _vendorConnectResponse.Content.ReadAsStringAsync();
                return _vendorConnectResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserProfileService::Following Exception occurred in UpdateUserProfile  \n" + ex.ToString());
                throw;
            }
        }


        #endregion

        #region Update communication preferences
        public async Task<HttpResponseMessage> UpdateCommunicationPreferences<T>(string profileId, CommunicationPreference_Request_DTO comPref_UpdateRequest, string vendorCode, string lobCode)
        {
             string vendorConnectUpdateComPrefUrl = UserProfileEndpoint.UpdateCommunicationPreferences(_vendorConnectBaseUrl, profileId, comPref_UpdateRequest.kind);
                _logger.LogDebug("[UpdateCommunicationPreferences] -> Calling {Uri} to update the user profile communication preferences", vendorConnectUpdateComPrefUrl);
                content = new StringContent(JsonConvert.SerializeObject(comPref_UpdateRequest), Encoding.UTF8, _applicationJson);
                // Adding headers
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Add("Vendor-Code", vendorCode);
                _apiClient.DefaultRequestHeaders.Add("Lob-Code", lobCode);
                _vendorConnectResponse = await _apiClient.PatchAsync(vendorConnectUpdateComPrefUrl, content);
                vendorConnectResponseContent = await _vendorConnectResponse.Content.ReadAsStringAsync();
                return _vendorConnectResponse;
           
        }
        #endregion


    }
}
