using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.Services.BlobStorageService
{
    public sealed class AzureBlobService : IBlobService
    {
        public const string AzureStorageConnectionString = nameof(AzureStorageConnectionString);
        public const string AzureStorageContainerName = nameof(AzureStorageContainerName);

        public const string DefaultContainerName = "items";

        private readonly string _connectionString;
        private readonly string _containerName;

        private readonly ILoggerService _loggerService;
        private readonly IRepositoryWrapper? _repositoryWrapper;

        public AzureBlobService(
            IConfiguration configuration,
            ILoggerService loggerService,
            IRepositoryWrapper? repositoryWrapper = null)
        {
            _connectionString = configuration.GetConnectionString(AzureStorageConnectionString)
                ?? throw new InvalidOperationException($"Connection string '{AzureStorageConnectionString}' not found.");
            _containerName = configuration[AzureStorageContainerName] ?? DefaultContainerName;

            _loggerService = loggerService;
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task CleanBlobStorageAsync(CancellationToken cancellationToken = default)
        {
            if (_repositoryWrapper is null)
            {
                return;
            }

            var existingImagesInDatabase = await _repositoryWrapper.ImageRepository.GetAllAsync();
            var existingAudiosInDatabase = await _repositoryWrapper.AudioRepository.GetAllAsync();

            var existingMedia = new List<string>();
            existingMedia.AddRange(existingImagesInDatabase.Select(image => image.BlobName!));
            existingMedia.AddRange(existingAudiosInDatabase.Select(audio => audio.BlobName!));

            var container = await GetOrCreateBlobContainerClientAsync(cancellationToken);
            var blobs = container.GetBlobsAsync(cancellationToken: cancellationToken);

            var existingMediaSet = existingMedia.ToHashSet();

            await foreach (var blob in blobs)
            {
                var blobName = blob.Name;
                if (existingMediaSet.Contains(blobName))
                {
                    _loggerService.LogInformation($"Deleting {blobName}...");
                    await container.DeleteBlobIfExistsAsync(blobName, cancellationToken: cancellationToken);
                }
            }
        }

        public string SaveFileInStorage(string base64, string name, string mimeType)
        {
            var bytes = Convert.FromBase64String(base64);
            var blobName = IBlobService.GenerateBlobName(name);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = mimeType
                },
            };

            var container = GetOrCreateBlobContainerClient();

            container.GetBlobClient(blobName).Upload(
                content: new MemoryStream(bytes),
                options: options);

            return blobName;
        }

        public async Task<string> SaveFileInStorageAsync(
            string base64,
            string name,
            string mimeType,
            CancellationToken cancellationToken = default)
        {
            var bytes = Convert.FromBase64String(base64);
            var blobName = IBlobService.GenerateBlobName(name);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = mimeType
                },
            };

            var container = await GetOrCreateBlobContainerClientAsync(cancellationToken);

            var response = await container.GetBlobClient($"{blobName}.{mimeType}")
                .UploadAsync(
                    content: new MemoryStream(bytes),
                    options: options,
                    cancellationToken: cancellationToken);

            return blobName;
        }

        public void DeleteFileInStorage(string name)
        {
            var container = GetOrCreateBlobContainerClient();
            container.DeleteBlobIfExists(name);
        }

        public async Task DeleteFileInStorageAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            var container = await GetOrCreateBlobContainerClientAsync(cancellationToken);
            await container.DeleteBlobIfExistsAsync(
                blobName: name,
                cancellationToken: cancellationToken);
        }

        public byte[] FindFileInStorageAsBytes(string name)
        {
            var container = GetOrCreateBlobContainerClient();
            var blob = container.GetBlobClient(name);

            var response = blob.DownloadContent();

            var bytes = response
                .Value
                .Content
                .ToArray();

            return bytes;
        }

        public async Task<byte[]> FindFileInStorageAsBytesAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            var container = await GetOrCreateBlobContainerClientAsync(cancellationToken);
            var blob = container.GetBlobClient(name);

            var response = await blob.DownloadContentAsync(cancellationToken: cancellationToken);
            var bytes = response.Value.Content.ToArray();

            return bytes;
        }

        private BlobContainerClient GetOrCreateBlobContainerClient()
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var container = blobServiceClient.GetBlobContainerClient(_containerName);

            container.CreateIfNotExists();

            return container;
        }

        private async Task<BlobContainerClient> GetOrCreateBlobContainerClientAsync(
            CancellationToken cancellationToken = default)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var container = blobServiceClient.GetBlobContainerClient(_containerName);

            await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            return container;
        }
    }
}
