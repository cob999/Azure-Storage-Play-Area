using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureStorageBlobPlayArea.Services
{
    public class ImageStorageService
    {
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly CloudBlobContainer _container;
        private readonly CloudBlockBlob _blob;
        private readonly Uri _blobServiceEndpoint = new Uri(ConfigurationManager.AppSettings.Get("blobServiceEndpoint"));

        public ImageStorageService()
        {
            var credentials = new StorageCredentials("storageplayarea", "i7+dG4heTsysr6oD+lNMRNni5Nr560ZY44TOKK7sRe0nQWaxUWovnlRY/a6mz3dPH1g1SWIrx2WQ9eu3mtpXLw==");
            _cloudBlobClient = new CloudBlobClient(_blobServiceEndpoint, credentials);
            _container = _cloudBlobClient.GetContainerReference("private-images");
        }

        public Uri GetResourceUri(string resourceId)
        {
            return new Uri(_blobServiceEndpoint, $"/images/{resourceId}");
        }

        public Uri GetResourceUriWithSas(string resourceId)
        {
            var sasPolicy = new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = DateTime.Now.AddMinutes(-10),
                SharedAccessExpiryTime = DateTime.Now.AddMinutes(30)
            };

            CloudBlockBlob blob = _container.GetBlockBlobReference(resourceId);
            string sasToken = blob.GetSharedAccessSignature(sasPolicy);

            return new Uri(_blobServiceEndpoint, $"/private-images/{resourceId}{sasToken}");
        }

        public async Task<string> SaveImage(Stream imageInputStream)
        {
            string id = Guid.NewGuid().ToString();
            CloudBlockBlob blob = _container.GetBlockBlobReference(id);

            await blob.UploadFromStreamAsync(imageInputStream);

            return id;
        }
    }
}