using Azure.Core.Pipeline;
using Azure.Storage.Blobs;

namespace ViLearning.Utility
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString, new BlobClientOptions
            {
                    Transport = new HttpClientTransport(new HttpClient { Timeout = TimeSpan.FromSeconds(102) }),
                    Retry = { NetworkTimeout = Timeout.InfiniteTimeSpan }
            });
        }
        public async Task<(Stream,string)> GetVideoStream(string containerName, string videoName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(videoName);
            var properties = await blobClient.GetPropertiesAsync();
            string contentType = properties.Value.ContentType;
            Stream stream = await blobClient.OpenReadAsync();
            return (stream, contentType);

        }
        public Task<string> GetPlaylistBlobName(string containerName, string playlist)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(playlist);
            return Task.FromResult(blobClient.Uri.ToString());
        }
        public async Task<string> UploadFileAsync(string containerName, string fileName, Stream fileStream)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, true);
            return blobClient.Uri.ToString();
        }

        public async Task DeleteFileAsync(string containerName, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }
    }
}
