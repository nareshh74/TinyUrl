namespace TinyUrl;

public interface IUrlRepository
{
    Task<bool> TryGetCodeAsync(Uri url, out string? code, CancellationToken _);
    Task<bool> TryGetUrlAsync(string code, out Uri url, CancellationToken _);
    Task<bool> TryAddUrlCodeMappingAsync(Uri url, string code, CancellationToken _);
}