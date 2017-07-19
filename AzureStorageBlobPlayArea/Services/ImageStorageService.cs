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
        private readonly Uri _blobServiceEndpoint = new Uri(ConfigurationManager.AppSettings.Get("blobServiceEndpoint"));

        public ImageStorageService()
        {
            var credentials = new StorageCredentials("storageplayarea", "Ve4n5rfKu/ys899/86EdEUS3xrXT66ZRzm0mhzQy2thCv9HNQ98RqEn1VWoHsQ8o9iozfG20zg2NW/248xiVQA==");
            _cloudBlobClient = new CloudBlobClient(_blobServiceEndpoint, credentials);    
        }

        public Uri GetResourceUri(string resourceId)
        {
            return new Uri(_blobServiceEndpoint, $"/images/{resourceId}");
        }

        public async Task<string> SaveImage(Stream imageInputStream)
        {
            string id = Guid.NewGuid().ToString();
            CloudBlobContainer container = _cloudBlobClient.GetContainerReference("images");
            CloudBlockBlob blob = container.GetBlockBlobReference(id);

            await blob.UploadFromStreamAsync(imageInputStream);

            return id;
        }
    }
}