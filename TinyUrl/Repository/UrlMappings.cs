namespace TinyUrl.Repository;

public sealed record UrlMappings
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Url { get; set; }
}