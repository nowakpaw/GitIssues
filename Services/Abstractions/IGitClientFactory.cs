using Shared.Enums;

namespace Services.Abstractions;

public interface IGitClientFactory
{
    IGitClient GetService(GitClientTypes type);
}