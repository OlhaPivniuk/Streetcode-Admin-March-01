using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.Services.BlobStorageService;

public sealed class BlobService : IBlobService
{
    private readonly string _keyCrypt;
    private readonly string _blobPath;
    private readonly ILoggerService _loggerService;
    private readonly IRepositoryWrapper? _repositoryWrapper;

    public BlobService(IOptions<BlobEnvironmentVariables> options, ILoggerService loggerService, IRepositoryWrapper? repositoryWrapper = null)
    {
        var variables = options.Value;
        _keyCrypt = variables.BlobStoreKey;
        _blobPath = variables.BlobStorePath;
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

        var existingMediaSet = existingMedia.ToHashSet();

        var blobNames = Directory.EnumerateFiles(_blobPath)
            .Select(path => Path.GetFileName(path));

        foreach (var blobName in blobNames)
        {
            if (existingMediaSet.Contains(blobName))
            {
                _loggerService.LogInformation($"Deleting {blobName}...");
                await DeleteFileInStorageAsync(blobName, cancellationToken);
            }
        }
    }

    public string SaveFileInStorage(string base64, string name, string mimeType)
    {
        var bytes = Convert.FromBase64String(base64);
        var blobName = IBlobService.GenerateBlobName(name);

        var encryptedBytes = EncryptBytes(bytes);

        Directory.CreateDirectory(_blobPath);
        File.WriteAllBytes($"{_blobPath}{name}.{mimeType}", encryptedBytes);

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

        var encryptedBytes = EncryptBytes(bytes);

        Directory.CreateDirectory(_blobPath);
        await File.WriteAllBytesAsync($"{_blobPath}{name}.{mimeType}", encryptedBytes, cancellationToken);

        return blobName;
    }

    public void DeleteFileInStorage(string name)
    {
        var path = $"{_blobPath}{name}";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public Task DeleteFileInStorageAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return Task.Run(() => DeleteFileInStorage(name), cancellationToken);
    }

    public byte[] FindFileInStorageAsBytes(
        string name)
    {
        var path = $"{_blobPath}{name}";

        var encryptedBytes = File.ReadAllBytes(path);
        var decryptedBytes = DecryptBytes(encryptedBytes);

        return decryptedBytes;
    }

    public async Task<byte[]> FindFileInStorageAsBytesAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        var path = $"{_blobPath}{name}";

        var encryptedBytes = await File.ReadAllBytesAsync(path, cancellationToken);
        var decryptedBytes = DecryptBytes(encryptedBytes);

        return decryptedBytes;
    }

    private byte[] EncryptBytes(byte[] bytes)
    {
        var keyBytes = Encoding.UTF8.GetBytes(_keyCrypt);
        var iv = new byte[16];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(iv);
        }

        byte[] encryptedBytes;
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.Key = keyBytes;
            aes.IV = iv;
            ICryptoTransform encryptor = aes.CreateEncryptor();
            encryptedBytes = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
        }

        var encryptedData = new byte[encryptedBytes.Length + iv.Length];
        Buffer.BlockCopy(iv, 0, encryptedData, 0, iv.Length);
        Buffer.BlockCopy(encryptedBytes, 0, encryptedData, iv.Length, encryptedBytes.Length);
        return encryptedData;
    }

    private byte[] DecryptBytes(byte[] bytes)
    {
        var keyBytes = Encoding.UTF8.GetBytes(_keyCrypt);

        var iv = new byte[16];
        Buffer.BlockCopy(bytes, 0, iv, 0, iv.Length);

        byte[] decryptedBytes;
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.Key = keyBytes;
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor();
            decryptedBytes = decryptor.TransformFinalBlock(bytes, iv.Length, bytes.Length - iv.Length);
        }

        return decryptedBytes;
    }
}