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
                                           var partialConfig = config.Build();

                                           var app = partialConfig.GetValue<string>("ApplicationName")
                                                     ?? hostingContext.HostingEnvironment.ApplicationName;

                                           var env = hostingContext.HostingEnvironment.EnvironmentName;

                                           var secretPrefix = $"{app}/{env}/";

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