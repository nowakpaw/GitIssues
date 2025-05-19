using Polly;
using Polly.Retry;
using System.Text;
using System.Text.Json;

namespace Services.Helpers;

public static class GitServiceHelper
{
    private static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy =
        Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => (int)r.StatusCode >= 500)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    public static async Task<string?> SendRequestAsync(
        HttpClient httpClient,
        Uri uri,
        HttpMethod method,
        object payload,
        Action<HttpRequestMessage> setHeaders,
        Func<string, string>? extractUrl,
        CancellationToken cancellationToken)
    {
        using var httpRequest = new HttpRequestMessage(method, uri)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };
        setHeaders(httpRequest);

        using var response = await RetryPolicy.ExecuteAsync(
            async ct => await httpClient.SendAsync(httpRequest, ct),
            cancellationToken);

        response.EnsureSuccessStatusCode();

        if (extractUrl is not null)
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return extractUrl(responseContent);
        }

        return null;
    }

    public static string ExtractIssueUrl(string responseContent, string propertyName)
    {
        using var doc = JsonDocument.Parse(responseContent);
        return doc.RootElement.TryGetProperty(propertyName, out var urlElement)
            ? urlElement.GetString() ?? string.Empty
            : string.Empty;
    }
}