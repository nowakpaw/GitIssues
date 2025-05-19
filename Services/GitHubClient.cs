using Microsoft.Extensions.Options;
using Services.Abstractions;
using Services.Helpers;
using Shared.Contracts.Requests.Issues;
using Shared.Enums;
using Shared.Options;
using System.Net.Http.Headers;

namespace Services;

public sealed class GitHubClient : IGitClient
{
    private readonly HttpClient _httpClient;
    private readonly GitServiceOptions _options;
    private const string IssueHtmlUrlResponseProperty = "html_url";
    private const string IssueCloseState = "closed";

    public GitClientTypes ClientType => GitClientTypes.GitHub;

    public GitHubClient(IHttpClientFactory httpClientFactory, IOptions<GitServicesOptions> options)
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
            new { title = request.Title, body = request.Description },
            SetDefaultHeaders,
            content => GitServiceHelper.ExtractIssueUrl(content, IssueHtmlUrlResponseProperty),
            cancellationToken
        )!;

    public Task<string> UpdateIssueAsync(UpdateIssueRequest request, CancellationToken cancellationToken)
        => GitServiceHelper.SendRequestAsync(
            _httpClient,
            BuildIssueUri(request.RepositoryOwner, request.RepositoryName, request.Id),
            HttpMethod.Patch,
            new { title = request.Title, body = request.Description },
            SetDefaultHeaders,
            content => GitServiceHelper.ExtractIssueUrl(content, IssueHtmlUrlResponseProperty),
            cancellationToken
        )!;

    public Task CloseIssueAsync(CloseIssueRequest request, CancellationToken cancellationToken)
        => GitServiceHelper.SendRequestAsync(
            _httpClient,
            BuildIssueUri(request.RepositoryOwner, request.RepositoryName, request.Id),
            HttpMethod.Patch,
            new { state = IssueCloseState },
            SetDefaultHeaders,
            null,
            cancellationToken
        );

    private Uri BuildIssueUri(string owner, string repo, int? issueId)
    {
        var idPart = issueId.HasValue ? $"/{issueId.Value}" : string.Empty;
        var path = $"/repos/{owner}/{repo}/issues{idPart}";
        return new Uri($"{_options.BaseUrl.TrimEnd('/')}{path}");
    }

    private void SetDefaultHeaders(HttpRequestMessage httpRequest)
    {
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.Token);
        httpRequest.Headers.UserAgent.ParseAdd("GitIssues/2025");
        httpRequest.Headers.Accept.ParseAdd("application/vnd.github+json");
    }
}