namespace TinyUrl;

public interface IUrlRepository
{
    Task<int> GenerateIdAsync(Uri url, CancellationToken cancellationToken);
    Task<bool> TryUpdateCodeAsync(int id, string code, CancellationToken cancellationToken);
    Task<Uri?> TryGetUrlByCodeAsync(string code, CancellationToken cancellationToken);
}