namespace UmkmJatim.Infrastructure.FileStorage;

public class FileStorageSettings
{
    public const string SectionName = "FileStorageSettings";

    public string UploadPath { get; set; } = "uploads";
    public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024; // 5MB
    public string[] AllowedMimeTypes { get; set; } = ["image/jpeg", "image/png", "image/webp"];
}
