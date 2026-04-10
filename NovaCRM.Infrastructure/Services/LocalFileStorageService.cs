using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NovaCRM.Application.Interfaces;

namespace NovaCRM.Infrastructure.Services;

public class LocalFileStorageService(
    IConfiguration config,
    ILogger<LocalFileStorageService> logger)
    : IFileStorageService
{
    private string RootPath =>
        config["FileStorageSettings:RootPath"] ?? "uploads";

    public async Task<string> SaveAsync(
        Guid customerId, string fileName, string contentType,
        Stream content, CancellationToken ct = default)
    {
        var ext        = Path.GetExtension(fileName);
        var storedName = $"{Guid.NewGuid()}{ext}";
        var dir        = Path.Combine(RootPath, customerId.ToString());

        Directory.CreateDirectory(dir);

        var fullPath = Path.Combine(dir, storedName);
        await using var fs = File.Create(fullPath);
        await content.CopyToAsync(fs, ct);

        logger.LogInformation(
            "Attachment saved: {StoredName} for customer {CustomerId} ({Bytes} bytes)",
            storedName, customerId, fs.Length);

        return storedName;
    }

    public Task<(Stream Content, string ContentType)?> GetAsync(
        Guid customerId, string storedName, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(RootPath, customerId.ToString(), storedName);

        if (!File.Exists(fullPath))
            return Task.FromResult<(Stream, string)?>(null);

        Stream stream = File.OpenRead(fullPath);

        var ext         = Path.GetExtension(storedName).ToLowerInvariant();
        var contentType = ext switch
        {
            ".pdf"  => "application/pdf",
            ".png"  => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif"  => "image/gif",
            ".txt"  => "text/plain",
            ".doc"  => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls"  => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _       => "application/octet-stream"
        };

        return Task.FromResult<(Stream, string)?>((stream, contentType));
    }

    public Task DeleteAsync(Guid customerId, string storedName, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(RootPath, customerId.ToString(), storedName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            logger.LogInformation(
                "Attachment deleted: {StoredName} for customer {CustomerId}",
                storedName, customerId);
        }

        return Task.CompletedTask;
    }
}
