using FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Request;
using FundingAccount.API.Application.Services.v1;
using FundingAccount.API.Domain.Exceptions;

internal sealed class Common_FundingAccountService : ICommonFundingAccountService
{
    private readonly HttpClient _apiClient;
    private readonly ILogger<Common_FundingAccountService> _logger;
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
    public Common_FundingAccountService(HttpClient httpClient, ILogger<Common_FundingAccountService> logger, IOptionsSnapshot<AppSettings> appSettings)
    {
        _apiClient = httpClient;
        _appSettings = appSettings.Value;
        _logger = logger;
        _vendorConnectBaseUrl = _appSettings.VendorConnectBaseUrl;
    }


    #region ValidateFundingAccount
    /// <summary>
    /// Get the request to validate funding account, forward it to the Vendor Connect Controller method
    /// </summary>
    /// <param name="fundingaccountDetails"></param>
    /// <returns>
    /// Receive the response from Vendor Connect Controller method and send it back to calling funding account controller method.
    /// </returns>


    public async Task<HttpResponseMessage> ValidateFundingAccount<T>(FA_Validate_Request_DTO fundingAccountRequest, string vendorCode, string lobCode)
    {
        _logger.LogInformation("FundingAccountService:: Funding account creation started.");
        var vendorConnectValidateFAUrl = APIEndpoints.ValidateFundingAccountVendorConnect(_vendorConnectBaseUrl);
        if(vendorConnectValidateFAUrl == null)
        {
            throw new BadRequestException(vendorCode, lobCode);
        }
        // Adding headers
        _apiClient.DefaultRequestHeaders.Clear();
        _apiClient.DefaultRequestHeaders.Add("Vendor-Code", vendorCode);
        _apiClient.DefaultRequestHeaders.Add("Lob-Code", lobCode);
        content = new StringContent(JsonConvert.SerializeObject(fundingAccountRequest), Encoding.UTF8, _applicationJson);
        _vendorConnectResponse = await _apiClient.PostAsync(vendorConnectValidateFAUrl, content);
        string _vendorConnectResponseContent = await _vendorConnectResponse.Content.ReadAsStringAsync();
        return _vendorConnectResponse;


    }

    #endregion
}

