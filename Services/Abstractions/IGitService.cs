namespace Services.Abstractions;

public interface IGitService
{
    Task<string> CreateIssueAsync(string title, string description, CancellationToken cancellationToken);
    Task UpdateIssueAsync(int issueId, string title, string description, CancellationToken cancellationToken);
    Task CloseIssueAsync(int issueId, CancellationToken cancellationToken);
}