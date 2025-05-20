namespace Shared.Options;

public class GitClientOptions
{
    public const string ConfigKey = "GitClients";
    public required string Name { get; set; }
    public required string BaseUrl { get; set; }
    public required string Token { get; set; }
}