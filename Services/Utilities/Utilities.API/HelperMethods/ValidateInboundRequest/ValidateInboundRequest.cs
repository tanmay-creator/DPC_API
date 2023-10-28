using Azure.Core;
using Azure.Identity;
using Azure.Storage.Files.DataLake;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Reflection;
using System.Text.Json;
using Utilities.API.Controllers.v1;

namespace Utilities.API.HelperMethods.ValidateInboundRequest
{
    public static class ValidateInboundRequest
    {
        public static IList<ValidationError> ValidateRequest(JsonDocument jsonRequest, string schemaName, ILogger<UtilitiesController> logger)
        {
            try
            {
                IList<ValidationError> errors = null;
                var fundingAcctJsonSchema = GetJsonSchema(schemaName, logger);
                var _requestSchema = JObject.Parse(jsonRequest.RootElement.ToString());               
                _requestSchema.IsValid(fundingAcctJsonSchema, out errors);
                return errors;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace, ex.Message);
                throw;
            }
        }

        private static JSchema GetJsonSchema(string schemaName, ILogger<UtilitiesController> logger)
        {
            try
            {
                logger.LogInformation("Reading the schema file {0}", schemaName);
                StreamReader schemaFilePath = System.IO.File.OpenText($"Schemas/{schemaName}");
                JsonTextReader textReader = new JsonTextReader(schemaFilePath);
                string path = Directory.GetCurrentDirectory();
                JSchemaUrlResolver resolver = new JSchemaUrlResolver();
                JSchema schemanew = JSchema.Load(textReader, new JSchemaReaderSettings
                {
                    Resolver = resolver,                   
                    BaseUri = new Uri($"file://{path}/Schemas/")
                });
                return schemanew;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace, ex.Message);
                throw;
            }
        }

        //public static IList<ValidationError> ValidateRequest(AzureADLSDetails settings, JsonDocument jsonRequest, string schemaName, ILogger<UtilitiesController> logger)
        //{
        //    try
        //    {
        //        IList<ValidationError> errors = null;
        //        var fundingAcctJsonSchema = GetJsonSchema(schemaName, settings, logger);
        //        var _requestSchema = JObject.Parse(jsonRequest.RootElement.ToString());
        //        //var _requestSchema = JObject.Parse(jsonRequest);
        //        _requestSchema.IsValid(fundingAcctJsonSchema, out errors);
        //        return errors;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex.StackTrace, ex.Message);
        //        throw;
        //    }
        //}

        //private static JSchema GetJsonSchema(string schemaName, AzureADLSDetails settings, ILogger<UtilitiesController> logger)
        //{
        //    try
        //    {
        //        // Schema stored in local folder
        //        //string filePath = $"Schemas/{schemaName}";
        //        //string jsonSchema = System.IO.File.ReadAllText(/*"JSON/{schemaname}.json"*/ filePath);
        //        //JSchema schema = JSchema.Parse(jsonSchema);
        //        //return schema;
        //        // For file reference in one of
        //        StreamReader file = System.IO.File.OpenText($"Schemas/{schemaName}");
        //        JsonTextReader reader = new JsonTextReader(file);
        //        string path = Directory.GetCurrentDirectory();
        //        JSchemaUrlResolver resolver = new JSchemaUrlResolver();
        //        JSchema schemanew = JSchema.Load(reader, new JSchemaReaderSettings
        //        {
        //            Resolver = resolver,
        //            // where the schema is being loaded from
        //            // referenced 'address.json' schema will be loaded from disk at
        //            //'c:\address.json'
        //            BaseUri = new Uri($"file://{path}/Schemas/")
        //        });
        //        return schemanew;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex.StackTrace, ex.Message);
        //        throw;
        //    }
        //}

        //private static JSchema GetRequestResponseSchema(string schemaName, ILogger<UtilitiesController> logger)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex.StackTrace, ex.Message);
        //        throw;
        //    }
        //}

        //private static JSchema GetJsonSchema(string schemaName, AzureADLSDetails settings, ILogger<UtilitiesController> logger)
        //{
        //    try
        //    {
        //        //Create a TokenCredential from service principal credentials
        //        var tokenCredential = CreateTokenCredential(settings);

        //        //Create a DataLakeServiceClient
        //        var serviceClient = CreateDataLakeServiceClient(settings.StorageAccountName, tokenCredential);

        //        //Get a DataLakeFileSystemClient
        //        var fileSystemClient = serviceClient.GetFileSystemClient(settings.FileSystemName);
        //        bool fileSystemExist = fileSystemClient.Exists();

        //        if (fileSystemExist)
        //        {

        //            // Get a DataLakeDirectoryClient
        //            DataLakeDirectoryClient directoryClient = fileSystemClient.GetDirectoryClient(settings.DirectoryName);
        //            bool directoryExist = directoryClient.Exists();

        //            if (directoryExist)
        //            {

        //                //Get data from a DataLakeFileClient
        //                DataLakeFileClient file = directoryClient.GetFileClient(schemaName);
        //                bool fileExist = file.Exists();

        //                if (fileExist)
        //                {
        //                    //Read data from file 
        //                    JSchema schema = GetSchemaFromFile(file);
        //                    return schema;
        //                }
        //                else
        //                {
        //                    //if file does not exist
        //                    throw new FileNotFoundException("DPC.404");
        //                }
        //            }
        //            else
        //            {
        //                //if directory does not exist
        //                throw new DirectoryNotFoundException("DPC.405");
        //            }
        //        }
        //        else
        //        {
        //            //if file system does not exist
        //            throw new FileNotFoundException("DPC.406");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex.StackTrace, ex.Message);
        //        throw;
        //    }
        //}

        //private static TokenCredential CreateTokenCredential(AzureADLSDetails settings)
        //{
        //    try
        //    {
        //        TokenCredential tokenCredential = new ClientSecretCredential(
        //           settings.TenantId,
        //           settings.ServicePrincipalClientId,
        //           settings.ServicePrincipalClientSecret);
        //        return tokenCredential;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //private static DataLakeServiceClient CreateDataLakeServiceClient(string storageAccountName, TokenCredential tokenCredential)
        //{
        //    try
        //    {
        //        DataLakeServiceClient serviceClient = new DataLakeServiceClient(
        //           new Uri($"https://{storageAccountName}.dfs.core.windows.net"),
        //           tokenCredential);
        //        return serviceClient;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //public static JSchema GetSchemaFromFile(DataLakeFileClient file)
        //{
        //    try
        //    {
        //        StreamReader data = new StreamReader(file.OpenRead());
        //        string text = data.ReadToEnd();
        //        JSchema schema = JSchema.Parse(text);
        //        return schema;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
    }
}
