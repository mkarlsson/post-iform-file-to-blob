using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace PostIFormFileToBlobStorage.Services
{
    public class BlobStorage 
    {
        CloudBlobClient _blobClient;
        CloudBlobContainer _container;

        public BlobStorage()
        {
            
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=NAMEOFSTORAGE;AccountKey=YOURSTORAGEKEY");

            // Create a blob client.
            _blobClient = storageAccount.CreateCloudBlobClient();
            _blobClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            // Get a reference to a container named “mycontainer.”
            _container = _blobClient.GetContainerReference("images");

            // If “mycontainer” doesn’t exist, create it.
            _container.CreateIfNotExistsAsync();
        }

        public async Task UploadAndSaveBlobAsync(IFormFile imageFile, string fileName)
        {
            
            await _container.CreateIfNotExistsAsync();
            await _container.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });
            //Get reference to a blob

            var blockBlob = _container.GetBlockBlobReference(fileName);
            blockBlob.Properties.ContentType = imageFile.ContentType;

            try
            {
                using (var stream = imageFile.OpenReadStream())
                {
                    await blockBlob.UploadFromStreamAsync(stream);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Could not upload image" + e.Message);
                throw;
            }

            var blobsName = blockBlob.Uri.ToString();
           
        }


        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public async Task<bool> AddToBlob(string folder, string name)
        {

            // Get a reference to a blob named "myblob".
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference("myblob");

            // Create or overwrite the "myblob" blob with the contents of a local file
            // named “myfile”.
            using (var fileStream = File.OpenRead(@"path\myfile"))
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }
            return true;
        }

    }
}