namespace FundingAccount.API.Utility.ReadRequest
{
    public static class ReadRequestBody
    {

        public static async Task<string> GetRequestRawBodyAsync(this HttpRequest httpJsonRequest, ILogger _logger, Encoding encoding = null)
        {
            try
            {

                _logger.LogInformation("\n Retrieve request body for funding account. ");

                if (!httpJsonRequest.Body.CanSeek)
                {
                    // We only do this if the stream isn't *already* seekable,
                    // as EnableBuffering will create a new stream instance
                    // each time it's called
                    httpJsonRequest.EnableBuffering();
                }
                
                var reader = new StreamReader(httpJsonRequest.Body, encoding ?? Encoding.UTF8);

                var body = await reader.ReadToEndAsync().ConfigureAwait(false);

                httpJsonRequest.Body.Position = 0;

                return body;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
