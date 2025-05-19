namespace Shared.Options;

public class GitServiceOptions
{
    public required string Name { get; set; }
    public required string BaseUrl { get; set; }
    public required string IssuesUrl { get; set; }
    public required string Token { get; set; }
}