using Microsoft.Extensions.Options;
using Shared.Contracts.Requests.Issues;
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

    public abstract Task<string> CreateIssueAsync(CreateIssueRequest request, CancellationToken cancellationToken);
    public abstract Task<string> UpdateIssueAsync(UpdateIssueRequest request, CancellationToken cancellationToken);
    public abstract Task CloseIssueAsync(CloseIssueRequest request, CancellationToken cancellationToken);
}