

using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace VendorConnect.API.Infrastructure.DPCCircuitBreaker
{
    public class DPCCircuitBreakerPolicy
    {
        private const int RETRY_COUNT = 3;
        private const int SLEEP_DURATION = 3;

        public AsyncRetryPolicy<HttpResponseMessage> ImmediateHttpRetry { get; }
        public AsyncRetryPolicy<HttpResponseMessage> LinearHttpRetry { get; }
        public AsyncRetryPolicy<HttpResponseMessage> ExponentialHttpRetry { get; }
        public AsyncCircuitBreakerPolicy<HttpResponseMessage> BasicCircuitBreakerPolicy { get; }
        public AsyncCircuitBreakerPolicy<HttpResponseMessage> AdvancedCircuitBreakerPolicy { get; }
        public AsyncPolicy<HttpResponseMessage> combinedPolicy { get; }
        public DPCCircuitBreakerPolicy()
        {
            ImmediateHttpRetry = Policy.HandleResult<HttpResponseMessage>(res => res.StatusCode == HttpStatusCode.Unauthorized
                                                                                   || res.StatusCode == HttpStatusCode.InternalServerError
                                                                                   || res.StatusCode == HttpStatusCode.ServiceUnavailable
                                                                                   )
                                             .WaitAndRetryAsync(RETRY_COUNT, retryAttempt => TimeSpan.FromSeconds(0),
                                             onRetry: (exception, SLEEP_DURATION, attemptNumber, context) =>
                                             {
                                                 Console.ForegroundColor = ConsoleColor.Red;
                                                 Console.WriteLine(DateTime.Now + $" Transient error in Immediate approach approach. {attemptNumber} / {RETRY_COUNT}");
                                             });
            //ImmediateHttpRetry = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
            //                                .WaitAndRetryAsync(RETRY_COUNT, retryAttempt => TimeSpan.FromSeconds(0),
            //                                onRetry: (exception, SLEEP_DURATION, attemptNumber, context) =>
            //                                {
            //                                    Console.ForegroundColor = ConsoleColor.Red;
            //                                    Console.WriteLine(DateTime.Now + $" Transient error in Immediate approach approach. {attemptNumber} / {RETRY_COUNT}");
            //                                });
            LinearHttpRetry = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
                                            .WaitAndRetryAsync(RETRY_COUNT, retryAttempt => TimeSpan.FromSeconds(SLEEP_DURATION),
                                            onRetry: (exception, SLEEP_DURATION, attemptNumber, context) =>
                                            {
                                                Console.ForegroundColor = ConsoleColor.Red;
                                                Console.WriteLine(DateTime.Now + $" Transient error in LinearHttpRetry approach. Retrying in {SLEEP_DURATION}. {attemptNumber} / {RETRY_COUNT}");
                                            });
            ExponentialHttpRetry = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
                                            .WaitAndRetryAsync(RETRY_COUNT, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                            onRetry: (exception, SLEEP_DURATION, attemptNumber, context) =>
                                            {
                                                Console.ForegroundColor = ConsoleColor.Red;
                                                Console.WriteLine(DateTime.Now + $" Transient error in ExponentialHttpRetry approach. Retrying in {SLEEP_DURATION}. {attemptNumber} / {RETRY_COUNT}");
                                            });
            BasicCircuitBreakerPolicy = Policy
                                          .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                                          .CircuitBreakerAsync(4, TimeSpan.FromSeconds(60), OnBreak, OnReset, OnHalfOpen);
            AdvancedCircuitBreakerPolicy = Policy
                                           .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                                           .AdvancedCircuitBreakerAsync(0.25, TimeSpan.FromSeconds(60), 7,
                                           TimeSpan.FromSeconds(30), OnBreak, OnReset, OnHalfOpen);

            combinedPolicy = Policy
                                .WrapAsync(ImmediateHttpRetry, BasicCircuitBreakerPolicy);
        }
        private void OnHalfOpen()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(DateTime.Now + " Circuit in test mode, one request will be allowed.");
        }

        private void OnReset()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(DateTime.Now + " Circuit closed, requests flow normally.");
        }

        private void OnBreak(DelegateResult<HttpResponseMessage> result, TimeSpan ts)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(DateTime.Now + " Circuit cut, requests will not flow.");
        }
    }
}
