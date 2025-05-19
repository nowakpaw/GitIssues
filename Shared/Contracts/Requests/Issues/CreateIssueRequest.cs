namespace Shared.Contracts.Requests.Issues;

public class CreateIssueRequest : BaseIssueRequest
{
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
}
