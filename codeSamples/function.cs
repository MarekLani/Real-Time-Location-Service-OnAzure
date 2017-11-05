using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;

namespace MessageProcessingFunction
{
    public static class MessageProcessor
    {
        [FunctionName("EventHubTriggerCSharp")]
        public static void Run([EventHubTrigger("spinhub", Connection = "EventHubConnectionString")]Message myEventHubMessage, [Blob("companysetups/{CompanyGuid}.json", Connection = "StorageAccountConnectionString")] Stream myInputBlob, TraceWriter log)
        {
            // get storage
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageAccountConnectionString"]);
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // get blob container
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(ConfigurationManager.AppSettings["StorageContainer"]);

            // get blob reference
            CloudBlob cloudBlob = blobContainer.GetBlobReference(myEventHubMessage.CompanyGuid + ".json");

            // get blob data
            if (!LocalEntities.Companies.ContainsKey(myEventHubMessage.CompanyGuid)
                || LocalEntities.Companies[myEventHubMessage.CompanyGuid] == null
                || cloudBlob.Properties.LastModified > LocalEntities.Companies[myEventHubMessage.CompanyGuid].LastModified)
            {
                Stream stream = new MemoryStream();
                cloudBlob.DownloadToStream(stream);
                stream.Position = 0;
                var sr = new StreamReader(stream);
                string json = sr.ReadToEnd();
                var c = JsonConvert.DeserializeObject<Company>(json);
                LocalEntities.Companies[myEventHubMessage.CompanyGuid] = new CompanySetting() { Company = c, LastModified = new DateTimeOffset(DateTime.Now) };
            }

            log.Info("Processing message");
            Core.MessageProcessor.ProcessMesssage(myEventHubMessage);
            log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");
        }
    }
}