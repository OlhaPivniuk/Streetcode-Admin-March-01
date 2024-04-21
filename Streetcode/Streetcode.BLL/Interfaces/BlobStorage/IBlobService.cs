using System.Security.Cryptography;
using System.Text;

namespace Streetcode.BLL.Interfaces.BlobStorage;

public interface IBlobService
{
    public Task CleanBlobStorageAsync(CancellationToken cancellationToken = default);

    public string SaveFileInStorage(
        string base64,
        string name,
        string mimeType);

    public Task<string> SaveFileInStorageAsync(
        string base64,
        string name,
        string mimeType,
        CancellationToken cancellationToken = default);

    public void DeleteFileInStorage(
        string name);

    public Task DeleteFileInStorageAsync(
        string name,
        CancellationToken cancellationToken = default);
    public string UpdateFileInStorage(
        string previousBlobName,
        string base64Format,
        string newBlobName,
        string extension)
        {
            DeleteFileInStorage(previousBlobName);
            return SaveFileInStorage(base64Format, newBlobName, extension);
        }

    public async Task<string> UpdateFileInStorageAsync(
        string previousBlobName,
        string base64Format,
        string newBlobName,
        string extension,
        CancellationToken cancellationToken = default)
    {
        await DeleteFileInStorageAsync(previousBlobName, cancellationToken);
        return await SaveFileInStorageAsync(base64Format, newBlobName, extension, cancellationToken);
    }

    public byte[] FindFileInStorageAsBytes(
        string name);

    public Task<byte[]> FindFileInStorageAsBytesAsync(
        string name,
        CancellationToken cancellationToken = default);

    public MemoryStream FindFileInStorageAsMemoryStream(
        string name)
    {
        var bytes = FindFileInStorageAsBytes(name);
        return new MemoryStream(bytes);
    }

    public async Task<MemoryStream> FindFileInStorageAsMemoryStreamAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        var bytes = await FindFileInStorageAsBytesAsync(name, cancellationToken);
        return new MemoryStream(bytes);
    }

    public string FindFileInStorageAsBase64(string name)
    {
        var bytes = FindFileInStorageAsBytes(name);
        return Convert.ToBase64String(bytes);
    }

    public async Task<string> FindFileInStorageAsBase64Async(
        string name,
        CancellationToken cancellationToken = default)
    {
        var bytes = await FindFileInStorageAsBytesAsync(name, cancellationToken);
        return Convert.ToBase64String(bytes);
    }

    public static string GenerateBlobName(string name)
    {
        var rawName = $"{DateTime.Now}{name}"
            .Replace(" ", "_")
            .Replace(".", "_")
            .Replace(":", "_");

        using var hash = SHA256.Create();

        Encoding enc = Encoding.UTF8;
        byte[] result = hash.ComputeHash(enc.GetBytes(rawName));

        return Convert.ToBase64String(result).Replace('/', '_');
    }
}
