using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Request.Unregistered;
using FundingAccount.API.Domain.Exceptions;

namespace FundingAccount.API.Application.Services.v1.FundingServices
{
    internal sealed class Unregistered_FundingAccountService : IUnregisteredFundingAccountService
    {
        private readonly HttpClient _apiClient;
        private readonly ILogger<Unregistered_FundingAccountService> _logger;
        private readonly string _vendorConnectBaseUrl;
        private AppSettings _appSettings { get; }

        private HttpResponseMessage _vendorConnectResponse;
        private HttpContent content;
        private readonly string _applicationJson = "application/json";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        /// <param name="fundingaccountRepository"></param>
        public Unregistered_FundingAccountService(IHttpClientFactory httpClientFactory, HttpClient httpClient, IOptions<AppSettings> settings, ILogger<Unregistered_FundingAccountService> logger, IOptionsSnapshot<AppSettings> appSettings, IConfiguration configuration/*, SecretClient secretClient*/)
        {
            _apiClient = httpClient;
            _appSettings = appSettings.Value;
            _logger = logger;
            _vendorConnectBaseUrl = _appSettings.VendorConnectBaseUrl;
        }

        #region Create Funding Account
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fundingaccountDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UnregisteredCardFundingAccount<T>(Unreg_FA_Card_Request_DTO fundingaccountDetails, string vendorCode, string lobCode)
        {

            _logger.LogInformation("FundingAccountService:: Funding account creation started.");
            var vendorConnectUnregCardUrl = APIEndpoints.CreateUnregCardFundingAccountVendorConnect(_vendorConnectBaseUrl);
            if (vendorConnectUnregCardUrl == null)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }
            // Adding headers
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Add("Vendor-Code", vendorCode);
            _apiClient.DefaultRequestHeaders.Add("Lob-Code", lobCode);
            content = new StringContent(JsonConvert.SerializeObject(fundingaccountDetails), Encoding.UTF8, _applicationJson);
            _vendorConnectResponse = await _apiClient.PostAsync(vendorConnectUnregCardUrl, content);
            string _vendorConnectResponseContent = await _vendorConnectResponse.Content.ReadAsStringAsync();
            return _vendorConnectResponse;


        }

        public async Task<HttpResponseMessage> UnregisteredACHFundingAccount<T>(Unreg_FA_ACH_Request_DTO fundingaccountDetails, string vendorCode, string lobCode)
        {

            _logger.LogInformation("FundingAccountService:: ACH Funding account creation started.");
            var vendorConnectUnregACHUrl = APIEndpoints.CreateUnregACHFundingAccountVendorConnect(_vendorConnectBaseUrl);
            if (vendorConnectUnregACHUrl == null)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }
            // Adding headers
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Add("Vendor-Code", vendorCode);
            _apiClient.DefaultRequestHeaders.Add("Lob-Code", lobCode);
            content = new StringContent(JsonConvert.SerializeObject(fundingaccountDetails), Encoding.UTF8, _applicationJson);
            _vendorConnectResponse = await _apiClient.PostAsync(vendorConnectUnregACHUrl, content);
            return _vendorConnectResponse;

        }

        #endregion


        #region Delete Funding Account
        public async Task<HttpResponseMessage> DeleteFundingAccount_UnregisteredUser<T>(string billerId, string billerAccountId, string fundingAccountId, string vendorCode, string lobCode)
        {

            _logger.LogInformation("FundingAccountService:: Funding account creation started.");
            string vendorConnectDeleteUnregUrl = APIEndpoints.DeleteUnregFundingAccountVendorConnect(_vendorConnectBaseUrl, billerId, billerAccountId, fundingAccountId);
            if (vendorConnectDeleteUnregUrl == null)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }
            // Adding headers
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Add("Vendor-Code", vendorCode);
            _apiClient.DefaultRequestHeaders.Add("Lob-Code", lobCode);
            _vendorConnectResponse = await _apiClient.DeleteAsync(vendorConnectDeleteUnregUrl);
            string _vendorConnectResponseContent = await _vendorConnectResponse.Content.ReadAsStringAsync();
            return _vendorConnectResponse;


        }


        #endregion
    }
}
