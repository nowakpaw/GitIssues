using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Options;

namespace Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGitClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GitServicesOptions>(configuration.GetSection(GitServicesOptions.ConfigKey));

        var gitServicesConfig = configuration.GetSection($"{GitServicesOptions.ConfigKey}:{nameof(GitServicesOptions.Services)}");
        var gitServices = gitServicesConfig.Get<List<GitServiceOptions>>() ?? new List<GitServiceOptions>();

        foreach (var service in gitServices)
        {
            services.AddHttpClient(service.Name, client =>
            {
                client.BaseAddress = new Uri(service.BaseUrl);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {service.Token}");
            });
        }

        return services;
    }
}