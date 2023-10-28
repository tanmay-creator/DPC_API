

//namespace FundingAccount.API.Utility.ValidateRequest
//{
//    public static class ValidateInboundRequest
//    {
//        public static IList<ValidationError> ValidateIncomingRequest(string jsonRequest, IOptions<AzureBlobDetails> settings) {

//            IList<ValidationError> errors = null;

//            var fundingAcctJsonSchema = GetJsonSchema(settings);
//            //string jsonSchema = System.IO.File.ReadAllText(schmemaPath);
//            //JSchema _jsonSchema = JSchema.Parse(jsonSchema);
//            //JObject _requestSchema = JObject.Parse(jsonRequest);
//            return errors;
//        }

//        public static async Task<string> GetJsonSchema(IOptions<AzureBlobDetails> settings)
//        {
//            string connectionString = $"{settings.Value.AzureBlobConnectionString}";

//            // Setup the connection to the storage account
//            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

//            // Connect to the blob storage
//            CloudBlobClient serviceClient = storageAccount.CreateCloudBlobClient();
//            // Connect to the blob container
//            CloudBlobContainer container = serviceClient.GetContainerReference($"{settings.Value.AzureBlobContainerName}");
//            // Connect to the blob file
//            CloudBlockBlob blob = container.GetBlockBlobReference($"{settings.Value.FundingAccountJsonSchema}");
//            // Get the blob file as text
//            string contents = blob.DownloadTextAsync().Result;

//            return contents;
//        }
//    }
//}
