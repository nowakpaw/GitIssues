namespace Shared.Options;

public class GitServicesOptions
{
    public static string ConfigKey => "GitServices";
    public List<GitServiceOptions> Services { get; set; } = [];
}