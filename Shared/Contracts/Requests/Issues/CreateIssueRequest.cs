using Shared.Enums;

namespace Shared.Contracts.Requests.Issues;

public sealed class CreateIssueRequest
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public GitServiceTypes GitServiceType { get; set; }
}
