using Microsoft.Extensions.Options;
using UmkmJatim.Application.Common.Interfaces;
using UmkmJatim.Domain.Exceptions;

namespace UmkmJatim.Infrastructure.FileStorage;

public class LocalFileStorageService : IFileStorage
{
    private readonly FileStorageSettings _settings;

    public LocalFileStorageService(IOptions<FileStorageSettings> options)
    {
        _settings = options.Value;
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default)
    {
        if (fileStream.Length > _settings.MaxFileSizeBytes)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "File", new[] { $"Ukuran file tidak boleh melebihi {_settings.MaxFileSizeBytes / (1024 * 1024)}MB." } }
            });
        }

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        if (!allowedExtensions.Contains(ext))
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "File", new[] { "Format file tidak didukung. Gunakan JPG, PNG, atau WebP." } }
            });
        }

        var targetFolder = Path.Combine(Directory.GetCurrentDirectory(), _settings.UploadPath, folder);
        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(targetFolder, uniqueFileName);

        using (var outputStream = new FileStream(fullPath, FileMode.Create))
        {
            await fileStream.CopyToAsync(outputStream, cancellationToken);
        }

        // Return URL path relative to server root
        return $"/uploads/{folder}/{uniqueFileName}";
    }

    public Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
            return Task.CompletedTask;

        var relativePath = fileUrl.TrimStart('/');
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}
