using Microsoft.Extensions.Options;
using Services.Abstractions;
using Shared.Contracts.Requests.Issues;
using Shared.Enums;
using Shared.Options;
using System.Text;
using System.Text.Json;

namespace Services;

public class GitLabService(
    IHttpClientFactory httpClientFactory,
    IOptions<GitServicesOptions> options
) : GitServiceBase(httpClientFactory, options, GitServiceTypes.GitLab)
{
    private const string ProjectIssuePath = "/projects/{0}/issues/{1}";

    public override async Task<string> CreateIssueAsync(CreateIssueRequest request, CancellationToken cancellationToken)
    {
        var projectId = Uri.EscapeDataString($"{request.RepositoryOwner}/{request.RepositoryName}");
        var uri = BuildIssueUri(projectId, null);
        var payload = new { title = request.Title, description = request.Description };

        using var httpRequest = BuildHttpRequest(uri, HttpMethod.Post, payload);

        using var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return ExtractIssueUrl(responseContent);
    }

    public override async Task<string> UpdateIssueAsync(UpdateIssueRequest request, CancellationToken cancellationToken)
    {
        var projectId = Uri.EscapeDataString($"{request.RepositoryOwner}/{request.RepositoryName}");
        var uri = BuildIssueUri(projectId, request.Id);
        var payload = new { title = request.Title, description = request.Description };

        using var httpRequest = BuildHttpRequest(uri, HttpMethod.Put, payload);

        using var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return ExtractIssueUrl(responseContent);
    }

    public override async Task CloseIssueAsync(CloseIssueRequest request, CancellationToken cancellationToken)
    {
        var projectId = Uri.EscapeDataString($"{request.RepositoryOwner}/{request.RepositoryName}");
        var uri = BuildIssueUri(projectId, request.Id);
        var payload = new { state_event = "close" };

        using var httpRequest = BuildHttpRequest(uri, HttpMethod.Put, payload);

        using var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private Uri BuildIssueUri(string projectId, int? issueIid)
    {
        var idPart = issueIid.HasValue ? issueIid.Value.ToString() : string.Empty;
        var path = string.Format(ProjectIssuePath, projectId, idPart).TrimEnd('/');
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
        httpRequest.Headers.Add("PRIVATE-TOKEN", Options.Token);
        httpRequest.Headers.UserAgent.ParseAdd("GitIssues/2025");
        httpRequest.Headers.Accept.ParseAdd("application/json");
    }

    private static string ExtractIssueUrl(string responseContent)
    {
        using var doc = JsonDocument.Parse(responseContent);
        return doc.RootElement.TryGetProperty("web_url", out var urlElement)
            ? urlElement.GetString() ?? string.Empty
            : string.Empty;
    }
}