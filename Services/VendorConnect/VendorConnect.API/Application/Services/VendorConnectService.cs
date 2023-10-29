using VendorConnect.API.Application.ModelDTOs.FundingAccount.Request.Unregistered;
using VendorConnect.API.Application.ModelDTOs.Payment.Request.Registered;
using VendorConnect.API.Application.Services.Services.Abstraction;
using VendorConnect.API.Infrastructure.DPCCircuitBreaker;
using static VendorConnect.API.Application.Services.APIEndpoint;

namespace VendorConnect.API.Services
{
    public class VendorConnectService : IVendorConnectService
    {
        #region Variables
        private readonly HttpClient _apiClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<VendorConnectService> _logger;
        private readonly string _fundingAccountUrl;
        private readonly string _xAuthKey;
        private readonly string _apimSubscriptionKey;
        private readonly string _apimGetTokenUrl;
        private readonly string _userProfileUrl;
        private readonly string _aciPaymentUrl;
        private readonly DPCCircuitBreakerPolicy _dpcCircuitBreakerPolicy;
        private  HttpResponseMessage aciResponse;
        private  string aciResponseContent;
        private AppSettings _appSettings { get; }
        private string _accessToken;

        #region Payment Variables
        private HttpResponseMessage _paymentAciResponse;
        #endregion

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        /// <param name="fundingaccountRepository"></param>
        public VendorConnectService(HttpClient httpClient, ILogger<VendorConnectService> logger
           , IOptionsSnapshot<AppSettings> appSettings, IConfiguration configuration, DPCCircuitBreakerPolicy dpcCircuitBreakerPolicy)
        {
            _apiClient = httpClient;
            _appSettings = appSettings.Value;
            _logger = logger;
            _configuration = configuration;
            _fundingAccountUrl = _appSettings.FundingAccountBaseUrl;
            _userProfileUrl = _appSettings.UserProfileBaseUrl;
            _aciPaymentUrl = _appSettings.ACIPaymentUrl;
            //_clientSecret = _appSettings.ClientSecret;
            _xAuthKey = _appSettings.XAuthKey;
            _apimSubscriptionKey = _appSettings.APIM_SubscriptionKey;
            _apimGetTokenUrl = _appSettings.ApimGetTokenUrl;
            _dpcCircuitBreakerPolicy = dpcCircuitBreakerPolicy;
        }

        private async Task<string> GetAccessToken()
        {

            _logger.LogInformation("VendorConnectAPIService:: GetAccessToken for the user profile from the ACI ");
            AuthResponse result = new AuthResponse();
            _apiClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apimSubscriptionKey);
            HttpResponseMessage apiResponse;
            apiResponse = await _dpcCircuitBreakerPolicy.combinedPolicy.ExecuteAsync(() => _apiClient.GetAsync(_apimGetTokenUrl));


            if (apiResponse.IsSuccessStatusCode)
            {
                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                if (apiContent != null)
                {
                    _accessToken = apiContent;
                    _logger.LogInformation("VendorConnectAPIService:: AccessToken obtained from the ACI is " + _accessToken);

                }
            }
            return _accessToken;

        }

        // Unregistered flow

        public async Task<HttpResponseMessage> UnregisteredCardFundingAccount(Unreg_FA_Card_Request_DTO fundingaccountDetails)
        {


            var uri = FundingAccountEndpoint.CreateFundingAccount(_fundingAccountUrl);
            //if(uri == null)
            //{
            //    throw new NotFoundException("aci url not found","Not");
            //}
            _logger.LogDebug("[CreateCardFundingAccount] -> Calling {Uri} to create the funding account", uri);

            //Fetch Access Token from ACI
            string ACCESSTOKEN = await GetAccessToken();
            if (!string.IsNullOrEmpty(ACCESSTOKEN))
            {
                HttpResponseMessage aciResponse = null;

                _logger.LogInformation("FundingAccountService:: Creating funding Account for the details: " + JsonConvert.SerializeObject(fundingaccountDetails));
                var json = JsonConvert.SerializeObject(fundingaccountDetails);
                var content = new StringContent(json, UTF8Encoding.UTF8, "application/json");
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Add("X-Auth-Key", _xAuthKey);
                _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" + ACCESSTOKEN);
                aciResponse = await _dpcCircuitBreakerPolicy.combinedPolicy.ExecuteAsync(() => _apiClient.PostAsync(uri, content));
                var aciResponseContent = await aciResponse.Content.ReadAsStringAsync();
                return aciResponse;

            }
            HttpResponseMessage errResponse = new HttpResponseMessage();
            errResponse.StatusCode = HttpStatusCode.NotFound;
            errResponse.Content = new StringContent("Access token is null");
            return errResponse;

        }

        public async Task<HttpResponseMessage> UnregisteredACHFundingAccount(Unreg_FA_ACH_Request_DTO fundingaccountDetails)
        {
            var uri = FundingAccountEndpoint.CreateFundingAccount(_fundingAccountUrl);
            _logger.LogDebug("[CreateACHFundingAccount] -> Calling {Uri} to create the funding account", uri);


            //Fetch Access Token from ACI
            string ACCESSTOKEN = await GetAccessToken();
            if (!string.IsNullOrEmpty(ACCESSTOKEN))
            {
                HttpResponseMessage aciResponse = null;

                _logger.LogInformation("VendorConnectService:: Creating funding Account for the details: " + JsonConvert.SerializeObject(fundingaccountDetails));
                var json = JsonConvert.SerializeObject(fundingaccountDetails);
                var content = new StringContent(json, UTF8Encoding.UTF8, "application/json");
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Add("X-Auth-Key", _xAuthKey);
                _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" + ACCESSTOKEN);
                aciResponse = await _dpcCircuitBreakerPolicy.combinedPolicy.ExecuteAsync(() => _apiClient.PostAsync(uri, content));
                var aciResponseContent = await aciResponse.Content.ReadAsStringAsync();
                return aciResponse;

            }
            HttpResponseMessage errResponse = new HttpResponseMessage();
            errResponse.StatusCode = HttpStatusCode.NotFound;
            errResponse.Content = new StringContent("Access token is null");
            return errResponse;

        }


        // Registered Flow
        public async Task<HttpResponseMessage> RegisteredCardFundingAccount(Reg_FA_Card_Request_DTO fundingaccountDetails)
        {
            var uri = FundingAccountEndpoint.CreateFundingAccount(_fundingAccountUrl);
            _logger.LogDebug("[CreateCardFundingAccount] -> Calling {Uri} to create the funding account", uri);



            //Fetch Access Token from ACI
            string ACCESSTOKEN = await GetAccessToken();
            if (!string.IsNullOrEmpty(ACCESSTOKEN))
            {
                HttpResponseMessage aciResponse = null;

                _logger.LogInformation("FundingAccountService:: Creating funding Account for the details: " + JsonConvert.SerializeObject(fundingaccountDetails));
                var json = JsonConvert.SerializeObject(fundingaccountDetails);
                var content = new StringContent(json, UTF8Encoding.UTF8, "application/json");
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Add("X-Auth-Key", _xAuthKey);
                _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" + ACCESSTOKEN);
                aciResponse = await _dpcCircuitBreakerPolicy.combinedPolicy.ExecuteAsync(() => _apiClient.PostAsync(uri, content));
                var aciResponseContent = await aciResponse.Content.ReadAsStringAsync();
                return aciResponse;

            }
            HttpResponseMessage errResponse = new HttpResponseMessage();
            errResponse.StatusCode = HttpStatusCode.NotFound;
            errResponse.Content = new StringContent("Access token is null");
            return errResponse;

        }

        public async Task<HttpResponseMessage> RegisteredACHFundingAccount(Reg_FA_ACH_Request_DTO fundingaccountDetails)
        {
            var uri = FundingAccountEndpoint.CreateFundingAccount(_fundingAccountUrl);
            _logger.LogDebug("[CreateACHFundingAccount] -> Calling {Uri} to create the funding account", uri);


            //Fetch Access Token from ACI
            string ACCESSTOKEN = await GetAccessToken();
            if (!string.IsNullOrEmpty(ACCESSTOKEN))
            {
                HttpResponseMessage aciResponse = null;

                _logger.LogInformation("VendorConnectService:: Creating funding Account for the details: " + JsonConvert.SerializeObject(fundingaccountDetails));
                var json = JsonConvert.SerializeObject(fundingaccountDetails);
                var content = new StringContent(json, UTF8Encoding.UTF8, "application/json");
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Add("X-Auth-Key", _xAuthKey);
                _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" + ACCESSTOKEN);
                aciResponse = await _dpcCircuitBreakerPolicy.combinedPolicy.ExecuteAsync(() => _apiClient.PostAsync(uri, content));
                var aciResponseContent = await aciResponse.Content.ReadAsStringAsync();
                return aciResponse;

            }
            HttpResponseMessage errResponse = new HttpResponseMessage();
            errResponse.StatusCode = HttpStatusCode.NotFound;
            errResponse.Content = new StringContent("Access token is null");
            return errResponse;

        }

        public async Task<HttpResponseMessage> GetAllFundingAccount(string profileId, string vendorConnect, string lobCode)
        {
            var uri = FundingAccountEndpoint.GetAllFundingAccountUrl(_fundingAccountUrl, profileId);
            _logger.LogDebug("[VendorConnect Service] -> Calling {Uri} to fetch all funding accounts", uri);

            //Fetch Access Token from ACI
            string ACCESSTOKEN = await GetAccessToken();
            if (!string.IsNullOrEmpty(ACCESSTOKEN))
            {
               

                _logger.LogInformation("VendorConnectService:: Fetching user profile for the profileId: ");
                //var json = JsonConvert.SerializeObject(profileId);
                //var content = new StringContent(json, UTF8Encoding.UTF8, "application/json");
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Add("X-Auth-Key", _xAuthKey);
                _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" + ACCESSTOKEN);
                aciResponse = await _dpcCircuitBreakerPolicy.combinedPolicy.ExecuteAsync(() => _apiClient.GetAsync(uri));
                aciResponseContent = await aciResponse.Content.ReadAsStringAsync();
                return aciResponse;

            }
            HttpResponseMessage errResponse = new HttpResponseMessage();
            errResponse.StatusCode = HttpStatusCode.NotFound;
            errResponse.Content = new StringContent("Access token is null");
            return errResponse;
        }

        public async Task<HttpResponseMessage> GetFundingAccount(string profileId, string fundingAccountId, string vendorConnect, string lobCode)
        {
            var uri = FundingAccountEndpoint.GetFundingAccountUrl(_fundingAccountUrl, profileId, fundingAccountId);
            _logger.LogDebug("[VendorConnect Service] -> Calling {Uri} to fetch the funding account", uri);

            //Fetch Access Token from ACI
            string ACCESSTOKEN = await GetAccessToken();
            if (!string.IsNullOrEmpty(ACCESSTOKEN))
            {
                

                _logger.LogInformation("VendorConnectService:: Fetching user profile for the profileId: ");
                //var json = JsonConvert.SerializeObject(profileId);
                //var content = new StringContent(json, UTF8Encoding.UTF8, "application/json");
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Add("X-Auth-Key", _xAuthKey);
                _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" + ACCESSTOKEN);
                aciResponse = await _dpcCircuitBreakerPolicy.combinedPolicy.ExecuteAsync(() => _apiClient.GetAsync(uri));
                aciResponseContent = await aciResponse.Content.ReadAsStringAsync();
                return aciResponse;

            }
            HttpResponseMessage errResponse = new HttpResponseMessage();
            errResponse.StatusCode = HttpStatusCode.NotFound;
            errResponse.Content = new StringContent("Access token is null");
            return errResponse;
        }

        #region Payment Services


        // Payment Creation

        /// <summary>
        /// Get the payment request from Vendor Connect Controller and call the ACI endpoint to make a payment for registered payer.
        /// Get the response from aci against that request and pass this response back to vendor controller.
        /// In case error return the appropiate message.
        /// </summary>
        /// <param name="unregisteredPaymentRequest"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<HttpResponseMessage> RegisteredPayment(Reg_Payment_Request_DTO registeredPaymentRequest)
        {


            var uri = PaymentEndpoint.MakePayment(_aciPaymentUrl);
            _logger.LogDebug("[MakePayment] -> Calling {0} to make the payment", uri);
            //Fetch Access Token from ACI
            string ACCESSTOKEN = await GetAccessToken();
            if (!string.IsNullOrEmpty(ACCESSTOKEN))
            {

                _logger.LogInformation("VendorConnectService:: Make Payment for the Unregistered Flow ");
                var json = JsonConvert.SerializeObject(registeredPaymentRequest);
                var content = new StringContent(json, UTF8Encoding.UTF8, "application/json");
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Add("X-Auth-Key", _xAuthKey);
                _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" + ACCESSTOKEN);
                _paymentAciResponse = await _dpcCircuitBreakerPolicy.combinedPolicy.ExecuteAsync(() => _apiClient.PostAsync(uri, content));
                var AciResponseContent = await _paymentAciResponse.Content.ReadAsStringAsync();

                return _paymentAciResponse;



            }
            HttpResponseMessage errResponse = new HttpResponseMessage();
            errResponse.StatusCode = HttpStatusCode.NotFound;
            errResponse.Content = new StringContent("Access Token Not Found");
            return errResponse;



        }


        /// <summary>
        /// Get the payment request from Vendor Connect Controller and call the ACI endpoint to make a payment for unregistered payer.
        /// Get the response from aci against that request and pass this response back to vendor controller.
        /// In case error return the appropiate message.
        /// </summary>
        /// <param name="unregisteredPaymentRequest"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<HttpResponseMessage> UnregisteredPayment(Unreg_Payment_Request_DTO unregisteredPaymentRequest)
        {


            var uri = PaymentEndpoint.MakePayment(_aciPaymentUrl);
            //var uri = "https://localhost:7141/Home/ReturnErrorResponse";
            _logger.LogDebug("[MakePayment] -> Calling {0} to make the payment", uri);
            //Fetch Access Token from ACI
            string ACCESSTOKEN = await GetAccessToken();
            if (!string.IsNullOrEmpty(ACCESSTOKEN))
            {

                _logger.LogInformation("VendorConnectService:: Make Payment for the Unregistered Flow ");
                var json = JsonConvert.SerializeObject(unregisteredPaymentRequest);
                var content = new StringContent(json, UTF8Encoding.UTF8, "application/json");
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Add("X-Auth-Key", _xAuthKey);
                _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" + ACCESSTOKEN);
                //AciResponse = await _dpcCircuitBreakerPolicy.combinedPolicy.ExecuteAsync(() => _apiClient.GetAsync(uri));

                _paymentAciResponse = await _dpcCircuitBreakerPolicy.combinedPolicy.ExecuteAsync(() => _apiClient.PostAsync(uri, content));

                var AciResponseContent = await _paymentAciResponse.Content.ReadAsStringAsync();

                return _paymentAciResponse;
            }
            HttpResponseMessage errResponse = new HttpResponseMessage();
            errResponse.StatusCode = HttpStatusCode.NotFound;
            errResponse.Content = new StringContent("Access Token Not Found");
            return errResponse;
        }


        #endregion

        // User Profile Flow
        public async Task<HttpResponseMessage> CreateUserProfile(UserProfile_Request_DTO userProfile_Request_DTO)
        {
            var uri = UserProfileEndpoint.CreateUserProfile(_userProfileUrl);
            _logger.LogDebug("[CreateUserProfile] -> Calling {Uri} to create the  user profile", uri);

            //Fetch Access Token from ACI
            string ACCESSTOKEN = await GetAccessToken();
            if (!string.IsNullOrEmpty(ACCESSTOKEN))
            {
                HttpResponseMessage aciResponse = null;

                _logger.LogInformation("VendorConnectService:: Creating user profile for the user: ");
                var json = JsonConvert.SerializeObject(userProfile_Request_DTO);
                var content = new StringContent(json, UTF8Encoding.UTF8, "application/json");
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Add("X-Auth-Key", _xAuthKey);
                _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" + ACCESSTOKEN);
                aciResponse = await _dpcCircuitBreakerPolicy.combinedPolicy.ExecuteAsync(() => _apiClient.PostAsync(uri, content));
                var aciResponseContent = await aciResponse.Content.ReadAsStringAsync();

                return aciResponse;

            }
            HttpResponseMessage errResponse = new HttpResponseMessage();
            errResponse.StatusCode = HttpStatusCode.NotFound;
            errResponse.Content = new StringContent("Access token is null");
            return errResponse;

        }

        public async Task<HttpResponseMessage> GetUserProfile(string profileId)
        {
            var uri = UserProfileEndpoint.GetUserProfile(_userProfileUrl, profileId);
            _logger.LogDebug("[GetUserProfile] -> Calling {Uri} to fetch the  user profile", uri);

            //Fetch Access Token from ACI
            string ACCESSTOKEN = await GetAccessToken();
            if (!string.IsNullOrEmpty(ACCESSTOKEN))
            {
                HttpResponseMessage aciResponse = null;

                _logger.LogInformation("VendorConnectService:: Fetching user profile for the profileId: ");
                //var json = JsonConvert.SerializeObject(profileId);
                //var content = new StringContent(json, UTF8Encoding.UTF8, "application/json");
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Add("X-Auth-Key", _xAuthKey);
                _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" + ACCESSTOKEN);
                aciResponse = await _dpcCircuitBreakerPolicy.combinedPolicy.ExecuteAsync(() => _apiClient.GetAsync(uri));
                string aciResponseContent = await aciResponse.Content.ReadAsStringAsync();
                return aciResponse;

            }
            HttpResponseMessage errResponse = new HttpResponseMessage();
            errResponse.StatusCode = HttpStatusCode.NotFound;
            errResponse.Content = new StringContent("Access token is null");
            return errResponse;

        }

        public async Task<HttpResponseMessage> UpdateUserProfile(string profileId, UserProfile_Request_DTO userProfile_UpdateRequest_DTO)
        {
            var uri = UserProfileEndpoint.UpdateUserProfile(_userProfileUrl, profileId);
            _logger.LogDebug("[UpdateUserProfile] -> Calling {Uri} to update the  user profile", uri);


            //Fetch Access Token from ACI
            string ACCESSTOKEN = await GetAccessToken();
            if (!string.IsNullOrEmpty(ACCESSTOKEN))
            {
                HttpResponseMessage aciResponse = null;

                _logger.LogInformation("VendorConnectService:: Updating user profile for the profileId: ");
                var json = JsonConvert.SerializeObject(userProfile_UpdateRequest_DTO);
                var content = new StringContent(json, UTF8Encoding.UTF8, "application/json");
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Add("X-Auth-Key", _xAuthKey);
                _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" + ACCESSTOKEN);
                aciResponse = await _dpcCircuitBreakerPolicy.combinedPolicy.ExecuteAsync(() => _apiClient.PatchAsync(uri, content));
                string aciResponseContent = await aciResponse.Content.ReadAsStringAsync();
                return aciResponse;

            }
            HttpResponseMessage errResponse = new HttpResponseMessage();
            errResponse.StatusCode = HttpStatusCode.NotFound;
            errResponse.Content = new StringContent("Access token is null");
            return errResponse;

        }

        // Delete unregistered funding account
        public async Task<HttpResponseMessage> DeleteUnregisteredFundingAccount(string billerId, string billerAccountId, string fundingAccountId)
        {
            var uri = FundingAccountEndpoint.DeleteUnregFundingAccount(_fundingAccountUrl, billerId, billerAccountId, fundingAccountId);
            _logger.LogDebug("[DeleteFundingAccount] -> Calling {Uri} to delete the funding account", uri);


            //Fetch Access Token from ACI
            string ACCESSTOKEN = await GetAccessToken();
            if (!string.IsNullOrEmpty(ACCESSTOKEN))
            {
                HttpResponseMessage aciResponse = null;

                _logger.LogInformation("VendorConnectService:: Fetching user profile for the profileId: ");
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Add("X-Auth-Key", _xAuthKey);
                _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" + ACCESSTOKEN);
                aciResponse = await _dpcCircuitBreakerPolicy.combinedPolicy.ExecuteAsync(() => _apiClient.DeleteAsync(uri));
                string aciResponseContent = await aciResponse.Content.ReadAsStringAsync();
                return aciResponse;

            }
            HttpResponseMessage errResponse = new HttpResponseMessage();
            errResponse.StatusCode = HttpStatusCode.NotFound;
            errResponse.Content = new StringContent("Access token is null");
            return errResponse;

        }

        public async Task<HttpResponseMessage> DeleteRegisteredFundingAccount(string profileId, string fundingAccountId)
        {
            var uri = FundingAccountEndpoint.DeleteRegFundingAccount(_fundingAccountUrl, profileId, fundingAccountId);
            _logger.LogDebug("[DeleteFundingAccount] -> Calling {Uri} to delete the funding account", uri);

            //Fetch Access Token from ACI
            string ACCESSTOKEN = await GetAccessToken();
            if (!string.IsNullOrEmpty(ACCESSTOKEN))
            {
                HttpResponseMessage aciResponse = null;

                _logger.LogInformation("VendorConnectService:: Deleting funding account for the profileId: ");
                //var json = JsonConvert.SerializeObject(profileId);
                //var content = new StringContent(json, UTF8Encoding.UTF8, "application/json");
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Add("X-Auth-Key", _xAuthKey);
                _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" + ACCESSTOKEN);
                aciResponse = await _dpcCircuitBreakerPolicy.combinedPolicy.ExecuteAsync(() => _apiClient.DeleteAsync(uri));
                string aciResponseContent = await aciResponse.Content.ReadAsStringAsync();
                return aciResponse;

            }
            HttpResponseMessage errResponse = new HttpResponseMessage();
            errResponse.StatusCode = HttpStatusCode.NotFound;
            errResponse.Content = new StringContent("Access token is null");
            return errResponse;

        }

        public async Task<HttpResponseMessage> UpdateCommunicationPreferences(string profileId, string kind, CommunicationPreference_Request_DTO comPrefRequest)
        {
            var uri = UserProfileEndpoint.UpdateCommunicationPreferences(_userProfileUrl, profileId, kind);
            _logger.LogDebug("[UpdateCommunicationPreferences] -> Calling {Uri} to update the  user profile communication", uri);
            
                //Fetch Access Token from ACI
                string ACCESSTOKEN = await GetAccessToken();
                if (!string.IsNullOrEmpty(ACCESSTOKEN))
                {
                    HttpResponseMessage aciResponse = null;
                   
                        _logger.LogInformation("VendorConnectService:: Updating user profile for the profileId: ");
                        var json = JsonConvert.SerializeObject(comPrefRequest);
                        var content = new StringContent(json, UTF8Encoding.UTF8, "application/json");
                        _apiClient.DefaultRequestHeaders.Clear();
                        _apiClient.DefaultRequestHeaders.Add("X-Auth-Key", _xAuthKey);
                        _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" + ACCESSTOKEN);
                        aciResponse = await _dpcCircuitBreakerPolicy.combinedPolicy.ExecuteAsync(() => _apiClient.PatchAsync(uri, content));
                        string aciResponseContent = await aciResponse.Content.ReadAsStringAsync();
                        return aciResponse;
                    
                }
                HttpResponseMessage errResponse = new HttpResponseMessage();
                errResponse.StatusCode = HttpStatusCode.NotFound;
                errResponse.Content = new StringContent("Access token is null");
                return errResponse;
           
        }

        // Validate funding account
        public async Task<HttpResponseMessage> ValidateFundingAccount(ValidateFA_RequestDTO fundingAccountRequest)
        {
            var uri = FundingAccountEndpoint.ValidateFundingAccount(_fundingAccountUrl);
            _logger.LogDebug("[ValidateFundingAccount] -> Calling {uri} to validate the funding account", uri);
            HttpResponseMessage aciResponse = null;
           
                //Fetch Access Token from ACI
                string ACCESSTOKEN = await GetAccessToken();
                if (!string.IsNullOrEmpty(ACCESSTOKEN))
                {
                   
                        _logger.LogInformation("FundingAccountService:: Validating funding Account for the details: ");
                        var json = JsonConvert.SerializeObject(fundingAccountRequest);
                        var content = new StringContent(json, UTF8Encoding.UTF8, "application/json");
                        _apiClient.DefaultRequestHeaders.Clear();
                        _apiClient.DefaultRequestHeaders.Add("X-Auth-Key", _xAuthKey);
                        _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" + ACCESSTOKEN);
                        aciResponse = await _dpcCircuitBreakerPolicy.combinedPolicy.ExecuteAsync(() => _apiClient.PostAsync(uri, content));
                        string aciResponseContent = await aciResponse.Content.ReadAsStringAsync();
                        return aciResponse;
                   
                }
                HttpResponseMessage errResponse = new HttpResponseMessage();
                errResponse.StatusCode = HttpStatusCode.NotFound;
                errResponse.Content = new StringContent("Access token is null");
                return errResponse;
           
        }
    }
}
