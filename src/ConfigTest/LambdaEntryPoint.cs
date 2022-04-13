using Amazon.Lambda.AspNetCoreServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ConfigTest;

public class LambdaEntryPoint : APIGatewayProxyFunction
{
    protected override void Init(IWebHostBuilder builder)
    {
        builder
            .UseStartup<Startup>();
    }

    protected override void Init(IHostBuilder builder)
    {
        builder
            .ConfigureAppConfiguration((hostingContext, config) =>
                                       {
                                           var env = hostingContext.HostingEnvironment.EnvironmentName;
                                           var app = hostingContext.HostingEnvironment.ApplicationName;
                                           var secretPrefix = $"{env}/{app}/";
                                           config.AddSecretsManager(configurator: opts =>
                                                                    {
                                                                        opts.SecretFilter =
                                                                            entry =>
                                                                                entry.Name
                                                                                    .StartsWith(secretPrefix);
                                                                        opts.KeyGenerator =
                                                                            (_, name) =>
                                                                                name[secretPrefix.Length..]
                                                                                    .Replace("__", ":");
                                                                    });
                                       });
    }
}