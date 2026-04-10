namespace NovaCRM.Application.Interfaces;

public interface IFileStorageService
{

    Task<string> SaveAsync(
        Guid   customerId,
        string fileName,
        string contentType,
        Stream content,
        CancellationToken ct = default);

    Task<(Stream Content, string ContentType)?> GetAsync(
        Guid   customerId,
        string storedName,
        CancellationToken ct = default);

    Task DeleteAsync(
        Guid   customerId,
        string storedName,
        CancellationToken ct = default);
}
