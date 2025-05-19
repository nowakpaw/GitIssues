using System.Text;
using System.Text.Json;

namespace Services.Helpers;

public static class GitServiceHelper
{
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

        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
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