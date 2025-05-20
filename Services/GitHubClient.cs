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
    private readonly GitClientOptions _options;
    private const string IssueHtmlUrlResponseProperty = "html_url";
    private const string IssueCloseState = "closed";

    public GitClients Client => GitClients.GitHub;

    public GitHubClient(IHttpClientFactory httpClientFactory, IOptions<List<GitClientOptions>> options)
    {
        var clientName = Client.ToString();
        _httpClient = httpClientFactory.CreateClient(clientName);
        _options = options.Value.First(s => s.Name == clientName);
    }

    public Task<string> CreateIssueAsync(CreateIssueRequest request, CancellationToken cancellationToken)
        => GitClientHelper.SendRequestAsync(
            _httpClient,
            BuildIssueUri(request.RepositoryOwner, request.RepositoryName, null),
            HttpMethod.Post,
            new { title = request.Title, body = request.Description },
            SetDefaultHeaders,
            content => GitClientHelper.ExtractIssueUrl(content, IssueHtmlUrlResponseProperty),
            cancellationToken
        )!;

    public Task<string> UpdateIssueAsync(UpdateIssueRequest request, CancellationToken cancellationToken)
        => GitClientHelper.SendRequestAsync(
            _httpClient,
            BuildIssueUri(request.RepositoryOwner, request.RepositoryName, request.Id),
            HttpMethod.Patch,
            new { title = request.Title, body = request.Description },
            SetDefaultHeaders,
            content => GitClientHelper.ExtractIssueUrl(content, IssueHtmlUrlResponseProperty),
            cancellationToken
        )!;

    public Task CloseIssueAsync(CloseIssueRequest request, CancellationToken cancellationToken)
        => GitClientHelper.SendRequestAsync(
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