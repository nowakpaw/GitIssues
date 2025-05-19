using Microsoft.Extensions.Options;
using Services.Abstractions;
using Shared.Contracts.Requests.Issues;
using Shared.Enums;
using Shared.Options;
using System.Net.Http.Headers;

namespace Services;

public sealed class GitHubService(
    IHttpClientFactory httpClientFactory,
    IOptions<GitServicesOptions> options
) : GitServiceBase(httpClientFactory, options, GitServiceTypes.GitHub)
{
    private const string RepoIssuesPath = "/repos/{0}/{1}/issues{2}";
    private const string IssueHtmlUrlResponseProperty = "html_url";
    private const string IssueCloseState = "closed";

    public override Task<string> CreateIssueAsync(CreateIssueRequest request, CancellationToken cancellationToken)
        => SendRequestAsync(
            BuildIssueUri(request.RepositoryOwner, request.RepositoryName, null),
            HttpMethod.Post,
            new { title = request.Title, body = request.Description },
            content => ExtractIssueUrl(content, IssueHtmlUrlResponseProperty),
            cancellationToken
        )!;

    public override Task<string> UpdateIssueAsync(UpdateIssueRequest request, CancellationToken cancellationToken)
        => SendRequestAsync(
            BuildIssueUri(request.RepositoryOwner, request.RepositoryName, request.Id),
            HttpMethod.Patch,
            new { title = request.Title, body = request.Description },
            content => ExtractIssueUrl(content, IssueHtmlUrlResponseProperty),
            cancellationToken
        )!;

    public override Task CloseIssueAsync(CloseIssueRequest request, CancellationToken cancellationToken)
        => SendRequestAsync(
            BuildIssueUri(request.RepositoryOwner, request.RepositoryName, request.Id),
            HttpMethod.Patch,
            new { state = IssueCloseState },
            null,
            cancellationToken
        );

    private Uri BuildIssueUri(string owner, string repo, int? issueId)
    {
        var idPart = issueId.HasValue ? $"/{issueId.Value}" : string.Empty;
        var path = string.Format(RepoIssuesPath, owner, repo, idPart);
        return new Uri($"{Options.BaseUrl.TrimEnd('/')}{path}");
    }

    protected override void SetDefaultHeaders(HttpRequestMessage httpRequest)
    {
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Options.Token);
        httpRequest.Headers.UserAgent.ParseAdd("GitIssues/2025");
        httpRequest.Headers.Accept.ParseAdd("application/vnd.github+json");
    }
}