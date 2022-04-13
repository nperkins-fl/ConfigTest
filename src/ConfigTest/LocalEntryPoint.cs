using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ConfigTest;

/// <summary>
///     The Main function can be used to run the ASP.NET Core application locally using the Kestrel webserver.
/// </summary>
public class LocalEntryPoint
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
        //           .ConfigureAppConfiguration((hostingContext, config) =>
        //                                      {
        //                                          var env = hostingContext.HostingEnvironment.EnvironmentName;
        //                                          var app = hostingContext.HostingEnvironment.ApplicationName;
        //                                          var secretPrefix = $"{env}/{app}/";
        //                                          config.AddSecretsManager(configurator: opts =>
        //                                                                   {
        //                                                                       opts.SecretFilter =
        //                                                                           entry =>
        //                                                                               entry.Name
        //                                                                                   .StartsWith(secretPrefix);
        //                                                                       opts.KeyGenerator =
        //                                                                           (_, name) =>
        //                                                                               name[secretPrefix.Length..]
        //                                                                                   .Replace("__", ":");
        //                                                                   });
        //                                      })
                   .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}