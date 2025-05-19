using Microsoft.Extensions.Options;
using Services.Abstractions;
using Shared.Enums;
using Shared.Options;

namespace Services;

public class GitLabService(IHttpClientFactory httpClientFactory, IOptions<GitServicesOptions> options) : GitServiceBase(
        httpClientFactory,
        options,
        GitServiceTypes.GitLab)
{
    public override async Task<string> CreateIssueAsync(string title, string description, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("todo request");
    }

    public override Task UpdateIssueAsync(int issueId, string title, string description, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public override Task CloseIssueAsync(int issueId, CancellationToken cancellationToken)
        => throw new NotImplementedException();
}