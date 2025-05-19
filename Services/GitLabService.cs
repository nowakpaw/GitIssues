using Microsoft.Extensions.Options;
using Services.Abstractions;
using Shared.Contracts.Requests.Issues;
using Shared.Enums;
using Shared.Options;

namespace Services;

public class GitLabService(
    IHttpClientFactory httpClientFactory,
    IOptions<GitServicesOptions> options
) : GitServiceBase(httpClientFactory, options, GitServiceTypes.GitLab)
{
    private const string ProjectIssuePath = "/projects/{0}/issues/{1}";
    private const string IssueHtmlUrlResponseProperty = "web_url";
    private const string IssueCloseState = "close";

    public override Task<string> CreateIssueAsync(CreateIssueRequest request, CancellationToken cancellationToken)
        => SendRequestAsync(
            BuildIssueUri(request.RepositoryOwner, request.RepositoryName, null),
            HttpMethod.Post,
            new { title = request.Title, description = request.Description },
            content => ExtractIssueUrl(content, IssueHtmlUrlResponseProperty),
            cancellationToken
        )!;

    public override Task<string> UpdateIssueAsync(UpdateIssueRequest request, CancellationToken cancellationToken)
        => SendRequestAsync(
            BuildIssueUri(request.RepositoryOwner, request.RepositoryName, request.Id),
            HttpMethod.Put,
            new { title = request.Title, description = request.Description },
            content => ExtractIssueUrl(content, IssueHtmlUrlResponseProperty),
            cancellationToken
        )!;

    public override Task CloseIssueAsync(CloseIssueRequest request, CancellationToken cancellationToken)
        => SendRequestAsync(
            BuildIssueUri(request.RepositoryOwner, request.RepositoryName, request.Id),
            HttpMethod.Put,
            new { state_event = IssueCloseState },
            null,
            cancellationToken
        );

    private Uri BuildIssueUri(string owner, string repo, int? issueIid)
    {
        var projectId = Uri.EscapeDataString($"{owner}/{repo}");
        var idPart = issueIid.HasValue ? issueIid.Value.ToString() : string.Empty;
        var path = string.Format(ProjectIssuePath, projectId, idPart).TrimEnd('/');
        return new Uri($"{Options.BaseUrl.TrimEnd('/')}{path}");
    }

    protected override void SetDefaultHeaders(HttpRequestMessage httpRequest)
    {
        httpRequest.Headers.Add("PRIVATE-TOKEN", Options.Token);
        httpRequest.Headers.Accept.ParseAdd("application/json");
    }
}