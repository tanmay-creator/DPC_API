namespace Transaction.API.Application.Services
{
    public static class APIEndpoint
    {
        public static class PaymentEndPoint
        {
            public static string MakeUnregisteredPaymentRequestToVendorConnect(string baseUri) => $"{baseUri}/UnregisteredPaymentRequestToVendorConnect";
            public static string MakeRegisteredPaymentRequestToVendorConnect(string baseUri) => $"{baseUri}/RegisteredPaymentRequestToVendorConnect";
        }
        public static class UtilityEndPoint
        {
            public static string ValidateInboundRequestInUtility(string baseUri, string schemaName, string vendorCode, string lobCode) => $"{baseUri}/v1/validate-inbound-request/{vendorCode}/{lobCode}?schema-name={schemaName}";

            public static string GetApiErrorDetails(string baseUri, string errorCategory, string vendorCode, string lobCode) => $"{baseUri}/v1/api-error-details/{vendorCode}/api-errors/{errorCategory}/{lobCode}";


        }

    }
}