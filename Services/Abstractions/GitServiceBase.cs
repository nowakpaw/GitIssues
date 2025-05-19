using Microsoft.Extensions.Options;
using Shared.Enums;
using Shared.Options;

namespace Services.Abstractions;

public abstract class GitServiceBase : IGitService
{
    protected readonly HttpClient HttpClient;
    protected readonly GitServiceOptions Options;

    protected GitServiceBase(
        IHttpClientFactory httpClientFactory,
        IOptions<GitServicesOptions> options,
        GitServiceTypes serviceType)
    {
        var serviceName = serviceType.ToString();
        HttpClient = httpClientFactory.CreateClient(serviceName);
        Options = options.Value.Services.First(s => s.Name == serviceName);
    }

    public abstract Task<string> CreateIssueAsync(string title, string description, CancellationToken cancellationToken);
    public abstract Task UpdateIssueAsync(int issueId, string title, string description, CancellationToken cancellationToken);
    public abstract Task CloseIssueAsync(int issueId, CancellationToken cancellationToken);
}