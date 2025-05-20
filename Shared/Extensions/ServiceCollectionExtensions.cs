using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Options;

namespace Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGitHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<List<GitClientOptions>>(configuration.GetSection(GitClientOptions.ConfigKey));

        var gitClients = configuration.GetSection(GitClientOptions.ConfigKey).Get<List<GitClientOptions>>() ?? new List<GitClientOptions>();

        foreach (var service in gitClients)
        {
            services.AddHttpClient(service.Name, client =>
            {
                client.BaseAddress = new Uri(service.BaseUrl);
            });
        }

        return services;
    }
}