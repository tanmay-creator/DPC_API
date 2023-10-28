using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Request;
using FundingAccount.API.Application.Services.v1;
using FundingAccount.API.Domain.Exceptions;
using System.Reflection.PortableExecutable;

internal sealed class Registered_FundingAccountService : IRegisteredFundingAccountService
{
    private readonly HttpClient _apiClient;
    private readonly ILogger<Registered_FundingAccountService> _logger;
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
    public Registered_FundingAccountService(IHttpClientFactory httpClientFactory, HttpClient httpClient, IOptions<AppSettings> settings, ILogger<Registered_FundingAccountService> logger, IOptionsSnapshot<AppSettings> appSettings, IConfiguration configuration/*, SecretClient secretClient*/)
    {
        _apiClient = httpClient;
        _appSettings = appSettings.Value;
        _logger = logger;
        _vendorConnectBaseUrl = _appSettings.VendorConnectBaseUrl;
    }

    #region Get All Funding Account

    public async Task<HttpResponseMessage> GetAllFundingAccount(string profileId, string vendorCode, string lobCode)
    {
        var vendorConnectGetAllFAUrl = APIEndpoints.GetAllFundingAccountVendorConnect(_vendorConnectBaseUrl, profileId);
         
        if (vendorConnectGetAllFAUrl == null)
        {
            throw new BadRequestException(vendorCode, lobCode);
        }

        // Adding headers
        _apiClient.DefaultRequestHeaders.Clear();
        _apiClient.DefaultRequestHeaders.Add("Vendor-Code", vendorCode);
        _apiClient.DefaultRequestHeaders.Add("Lob-Code", lobCode);
        _vendorConnectResponse = await _apiClient.GetAsync(vendorConnectGetAllFAUrl);
        return _vendorConnectResponse;

    }
    #endregion

    #region Create Funding Account

    public async Task<HttpResponseMessage> RegisteredCardFundingAccount<T>(Reg_FA_Card_Request_DTO fundingaccountDetails, string vendorCode, string lobCode)
    {

        _logger.LogInformation("FundingAccountService:: Registered card Funding account creation started.");
        var vendorConnectCreateRegCardFAUrl = APIEndpoints.CreateRegCardFundingAccountVendorConnect(_vendorConnectBaseUrl);
        if (vendorConnectCreateRegCardFAUrl == null)
        {
            throw new BadRequestException(vendorCode, lobCode);
        }
        // Adding headers
        _apiClient.DefaultRequestHeaders.Clear();
        _apiClient.DefaultRequestHeaders.Add("Vendor-Code", vendorCode);
        _apiClient.DefaultRequestHeaders.Add("Lob-Code", lobCode);
        content = new StringContent(JsonConvert.SerializeObject(fundingaccountDetails), Encoding.UTF8, _applicationJson);
        _vendorConnectResponse = await _apiClient.PostAsync(vendorConnectCreateRegCardFAUrl, content);
        return _vendorConnectResponse;


    }

    public async Task<HttpResponseMessage> RegisteredACHFundingAccount<T>(Reg_FA_ACH_Request_DTO fundingaccountDetails, string vendorCode, string lobCode)
    {
        _logger.LogInformation("FundingAccountService:: Registered ACH Funding account creation started.");
        var vendorConnectRegACHUrl = APIEndpoints.CreateRegACHFundingAccountVendorConnect(_vendorConnectBaseUrl);
        if (vendorConnectRegACHUrl == null)
        {
            throw new BadRequestException(vendorCode, lobCode);
        }
        // Adding headers
        _apiClient.DefaultRequestHeaders.Clear();
        _apiClient.DefaultRequestHeaders.Add("Vendor-Code", vendorCode);
        _apiClient.DefaultRequestHeaders.Add("Lob-Code", lobCode);
        content = new StringContent(JsonConvert.SerializeObject(fundingaccountDetails), Encoding.UTF8, _applicationJson);
        _vendorConnectResponse = await _apiClient.PostAsync(vendorConnectRegACHUrl, content);
        return _vendorConnectResponse;
    }


    #endregion


    #region Delete Funding Account

    public async Task<HttpResponseMessage> DeleteFundingAccount_RegisteredUser<T>(string profileId, string fundingAccountId, string vendorCode, string lobCode)
    {

        _logger.LogInformation("FundingAccountService:: Funding account creation started.");
        var vendorConnectDeleteRegUrl = APIEndpoints.DeleteRegFundingAccountVendorConnect(_vendorConnectBaseUrl, profileId, fundingAccountId);
        if (vendorConnectDeleteRegUrl == null)
        {
            throw new BadRequestException(vendorCode, lobCode);
        }
        // Adding headers
        _apiClient.DefaultRequestHeaders.Clear();
        _apiClient.DefaultRequestHeaders.Add("Vendor-Code", vendorCode);
        _apiClient.DefaultRequestHeaders.Add("Lob-Code", lobCode);
        _vendorConnectResponse = await _apiClient.DeleteAsync(vendorConnectDeleteRegUrl);
        string _vendorConnectResponseContent = await _vendorConnectResponse.Content.ReadAsStringAsync();
        return _vendorConnectResponse;


    }
}
        #endregion






