using Newtonsoft.Json;
using Transaction.API.Application.ModelDTOs.v1.Payment.Request.Unregistered;
using Transaction.API.Application.Services.v1.Services.Abstraction;
using Transaction.API.Domain.Exceptions;
using static Transaction.API.Application.Services.APIEndpoint;

namespace Transaction.API.Application.Services.v1.PaymentsServices
{
    internal sealed class UnregisteredPaymentService : IUnregisteredPaymentService
    {
        #region Variables
        private readonly HttpClient _apiClient;
        private readonly ILogger<UnregisteredPaymentService> _logger;
        private readonly string _vendorConnectBaseUrl;
        private HttpResponseMessage _unregAciResponse;
        private HttpResponseMessage _regAciResponse;
        private AppSettings _appSettings { get; }
        #endregion

        public UnregisteredPaymentService(IHttpClientFactory httpClientFactory, HttpClient httpClient, IOptions<AppSettings> settings, ILogger<UnregisteredPaymentService> logger,
            IOptionsSnapshot<AppSettings> appSettings, IConfiguration configuration)
        {
            _apiClient = httpClient;
            _appSettings = appSettings.Value;
            _logger = logger;
            _vendorConnectBaseUrl = _appSettings.VendorConnectBaseUrl;
        }
        /// <summary>
        /// Get unregistered payment request from controller and call the Vendor Connect Microservice endpoint to make a payment.
        /// Get the response back fom Vendor Connect Microservice and will pass the response to payment controller method.
        /// In case of error response back to Payment Controller
        /// </summary>
        /// <param name="payment_req"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> MakeUnregisteredPayment(Unreg_Payment_Request_DTO payment_req, string vendorCode, string lobCode)
        {

            _logger.LogInformation("Payment Microservice:: MakeUnregisteredPayment Service Started.");
            string vendorConnectMakeUnregPaymentUrl = PaymentEndPoint.MakeUnregisteredPaymentRequestToVendorConnect(_vendorConnectBaseUrl);
            if (vendorConnectMakeUnregPaymentUrl == null)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }
            _apiClient.DefaultRequestHeaders.Clear();
            HttpContent content = new StringContent(JsonConvert.SerializeObject(payment_req), Encoding.UTF8, "application/json");
            // Adding headers
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Add("Vendor-Code", vendorCode);
            _apiClient.DefaultRequestHeaders.Add("Lob-Code", lobCode);

            _unregAciResponse = await _apiClient.PostAsync(vendorConnectMakeUnregPaymentUrl, content);
            return _unregAciResponse;

        }


    }
}