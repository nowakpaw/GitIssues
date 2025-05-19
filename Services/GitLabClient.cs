using Microsoft.Extensions.Options;
using Services.Abstractions;
using Services.Helpers;
using Shared.Contracts.Requests.Issues;
using Shared.Enums;
using Shared.Options;

namespace Services;

public sealed class GitLabClient : IGitClient
{
    private readonly HttpClient _httpClient;
    private readonly GitServiceOptions _options;
    private const string IssueHtmlUrlResponseProperty = "web_url";
    private const string IssueCloseState = "close";

    public GitClientTypes ClientType => GitClientTypes.GitLab;

    public GitLabClient(IHttpClientFactory httpClientFactory, IOptions<GitServicesOptions> options)
    {
        var serviceName = ClientType.ToString();
        _httpClient = httpClientFactory.CreateClient(serviceName);
        _options = options.Value.Services.First(s => s.Name == serviceName);
    }

    public Task<string> CreateIssueAsync(CreateIssueRequest request, CancellationToken cancellationToken)
        => GitServiceHelper.SendRequestAsync(
            _httpClient,
            BuildIssueUri(request.RepositoryOwner, request.RepositoryName, null),
            HttpMethod.Post,
            new { title = request.Title, description = request.Description },
            SetDefaultHeaders,
            content => GitServiceHelper.ExtractIssueUrl(content, IssueHtmlUrlResponseProperty),
            cancellationToken
        )!;

    public Task<string> UpdateIssueAsync(UpdateIssueRequest request, CancellationToken cancellationToken)
        => GitServiceHelper.SendRequestAsync(
            _httpClient,
            BuildIssueUri(request.RepositoryOwner, request.RepositoryName, request.Id),
            HttpMethod.Put,
            new { title = request.Title, description = request.Description },
            SetDefaultHeaders,
            content => GitServiceHelper.ExtractIssueUrl(content, IssueHtmlUrlResponseProperty),
            cancellationToken
        )!;

    public Task CloseIssueAsync(CloseIssueRequest request, CancellationToken cancellationToken)
        => GitServiceHelper.SendRequestAsync(
            _httpClient,
            BuildIssueUri(request.RepositoryOwner, request.RepositoryName, request.Id),
            HttpMethod.Put,
            new { state_event = IssueCloseState },
            SetDefaultHeaders,
            null,
            cancellationToken
        );

    private Uri BuildIssueUri(string owner, string repo, int? issueIid)
    {
        var projectId = Uri.EscapeDataString($"{owner}/{repo}");
        var idPart = issueIid.HasValue ? issueIid.Value.ToString() : string.Empty;
        var path = $"/projects/{projectId}/issues/{idPart}".TrimEnd('/');
        return new Uri($"{_options.BaseUrl.TrimEnd('/')}{path}");
    }

    private void SetDefaultHeaders(HttpRequestMessage httpRequest)
    {
        httpRequest.Headers.Add("PRIVATE-TOKEN", _options.Token);
        httpRequest.Headers.Accept.ParseAdd("application/json");
    }
}