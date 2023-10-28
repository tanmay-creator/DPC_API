using Newtonsoft.Json;
using Transaction.API.Application.ModelDTOs.v1.Payment.Request.Registered;
using Transaction.API.Application.Services.v1.Services.Abstraction;
using static Transaction.API.Application.Services.APIEndpoint;

namespace Transaction.API.Application.Services.v1.PaymentsServices
{
    internal sealed class RegisteredPaymentService : IRegisteredPaymentService
    {
        #region Variables
        private readonly HttpClient _apiClient;
        private readonly ILogger<RegisteredPaymentService> _logger;
        private readonly string _vendorConnectBaseUrl;
        private HttpResponseMessage _unregAciResponse;
        private HttpResponseMessage _regAciResponse;
        private AppSettings _appSettings { get; }
        #endregion

        public RegisteredPaymentService(IHttpClientFactory httpClientFactory, HttpClient httpClient, IOptions<AppSettings> settings, ILogger<RegisteredPaymentService> logger,
            IOptionsSnapshot<AppSettings> appSettings, IConfiguration configuration)
        {
            _apiClient = httpClient;
            _appSettings = appSettings.Value;
            _logger = logger;
            _vendorConnectBaseUrl = _appSettings.VendorConnectBaseUrl;
        }
      

        /// <summary>
        /// Get registered payment request from controller and call the Vendor Connect Microservice endpoint to make a payment.
        /// Get the response back fom Vendor Connect Microservice and will pass the response to payment controller method.
        /// In case of error response back to Payment Controller
        /// </summary>
        /// <param name="payment_req"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> MakeRegisteredPayment(Reg_Payment_Request_DTO payment_req, string vendorCode, string lobCode
)
        {
            try
            {
                _logger.LogInformation("Payment Microservice:: MakeRegisteredPayment Service Started.");
                string vendorConnectMakeUnregPaymentUrl = PaymentEndPoint.MakeRegisteredPaymentRequestToVendorConnect(_vendorConnectBaseUrl);
                _apiClient.DefaultRequestHeaders.Clear();
                HttpContent content = new StringContent(JsonConvert.SerializeObject(payment_req), Encoding.UTF8, "application/json");
                // Adding headers
                _apiClient.DefaultRequestHeaders.Clear();
                _apiClient.DefaultRequestHeaders.Add("Vendor-Code", vendorCode);
                _apiClient.DefaultRequestHeaders.Add("Lob-Code", lobCode);

                _regAciResponse = await _apiClient.PostAsync(vendorConnectMakeUnregPaymentUrl, content);
                var aciResponseContent = await _regAciResponse.Content.ReadAsStringAsync();
                return _regAciResponse;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Payment Microservice:: Error in MakeUnregisteredPayment Service.");
                HttpResponseMessage errResponse = new HttpResponseMessage();
                errResponse.StatusCode = HttpStatusCode.InternalServerError;
                errResponse.Content = new StringContent(ex.Message);
                return errResponse;
            }
        }
    }
}