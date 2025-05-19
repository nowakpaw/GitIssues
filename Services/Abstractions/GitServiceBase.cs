using Microsoft.Extensions.Options;
using Shared.Contracts.Requests.Issues;
using Shared.Enums;
using Shared.Options;
using System.Text;
using System.Text.Json;

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

    protected async Task<string?> SendRequestAsync(
        Uri uri,
        HttpMethod method,
        object payload,
        Func<string, string>? extractUrl,
        CancellationToken cancellationToken)
    {
        using var httpRequest = new HttpRequestMessage(method, uri)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };
        SetDefaultHeaders(httpRequest);

        using var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        if (extractUrl is not null)
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return extractUrl(responseContent);
        }

        return null;
    }

    protected virtual void SetDefaultHeaders(HttpRequestMessage httpRequest) { }

    protected static string ExtractIssueUrl(string responseContent, string propertyName)
    {
        using var doc = JsonDocument.Parse(responseContent);
        return doc.RootElement.TryGetProperty(propertyName, out var urlElement)
            ? urlElement.GetString() ?? string.Empty
            : string.Empty;
    }
}