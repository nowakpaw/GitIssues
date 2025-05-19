using Microsoft.Extensions.Options;
using Services.Abstractions;
using Shared.Contracts.Requests.Issues;
using Shared.Enums;
using Shared.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Services;

public class GitHubService(
    IHttpClientFactory httpClientFactory,
    IOptions<GitServicesOptions> options
) : GitServiceBase(httpClientFactory, options, GitServiceTypes.GitHub)
{
    private const string RepoIssuesPath = "/repos/{0}/{1}/issues{2}";

    public override async Task<string> CreateIssueAsync(CreateIssueRequest request, CancellationToken cancellationToken)
    {
        var uri = BuildIssueUri(request.RepositoryOwner, request.RepositoryName, null);
        var payload = new { title = request.Title, body = request.Description };
        using var httpRequest = BuildHttpRequest(uri, HttpMethod.Post, payload);

        using var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return ExtractIssueUrl(responseContent);
    }

    public override async Task<string> UpdateIssueAsync(UpdateIssueRequest request, CancellationToken cancellationToken)
    {
        var uri = BuildIssueUri(request.RepositoryOwner, request.RepositoryName, request.Id);
        var payload = new { title = request.Title, body = request.Description };
        using var httpRequest = BuildHttpRequest(uri, HttpMethod.Patch, payload);

        using var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return ExtractIssueUrl(responseContent);
    }

    public override async Task CloseIssueAsync(CloseIssueRequest request, CancellationToken cancellationToken)
    {
        var uri = BuildIssueUri(request.RepositoryOwner, request.RepositoryName, request.Id);
        var payload = new { state = "closed" };
        using var httpRequest = BuildHttpRequest(uri, HttpMethod.Patch, payload);

        using var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private Uri BuildIssueUri(string owner, string repo, int? issueId)
    {
        var idPart = issueId.HasValue ? $"/{issueId.Value}" : string.Empty;
        var path = string.Format(RepoIssuesPath, owner, repo, idPart);
        return new Uri($"{Options.BaseUrl.TrimEnd('/')}{path}");
    }

    private HttpRequestMessage BuildHttpRequest(Uri uri, HttpMethod method, object payload)
    {
        var httpRequest = new HttpRequestMessage(method, uri)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };
        SetDefaultHeaders(httpRequest);
        return httpRequest;
    }

    private void SetDefaultHeaders(HttpRequestMessage httpRequest)
    {
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Options.Token);
        httpRequest.Headers.UserAgent.ParseAdd("GitIssues/2025");
        httpRequest.Headers.Accept.ParseAdd("application/vnd.github+json");
    }

    private static string ExtractIssueUrl(string responseContent)
    {
        using var doc = JsonDocument.Parse(responseContent);
        return doc.RootElement.TryGetProperty("html_url", out var urlElement)
            ? urlElement.GetString() ?? string.Empty
            : string.Empty;
    }
}