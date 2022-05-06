using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.SecretsManager;
using Constructs;
using Stack = Amazon.CDK.Stack;

namespace ConfigTest.Deploy;

public class ConfigTestStack : Stack
{
    public ConfigTestStack(Construct scope, string id, IStackProps props)
        : base(scope, id, props)
    {
        var environmentName = System.Environment.GetEnvironmentVariable("TARGET_ENVIRONMENT") ?? "dev";
        var environmentVariables = EnvironmentVariablesFactory.AddEnvironmentVariables(this, environmentName);
        environmentVariables.Add("Example__Setting2", "From CDK");

        var function =
            new DockerImageFunction(this,
                                    "ConfigTestLambdas",
                                    new DockerImageFunctionProps
                                    {
                                        Code =
                                            DockerImageCode.FromImageAsset(@"src/ConfigTest"),
                                        Description = ".Net 6 Lambda to test reading config values",
                                        Environment = environmentVariables,
                                        Timeout = Duration.Minutes(5)
                                    });

        function.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps
                                                     {
                                                         Actions = new []
                                                                   {
                                                                       "secretsmanager:ListSecrets"//,
                                                                       //"kms:Encrypt",
                                                                       //"kms:Decrypt"
                                                                   },
                                                         Resources = new []{"*"}
                                                     }));

        var table = Table.FromTableName(this, "ExampleTable", "ExampleTable");

        table.GrantReadWriteData(function);

        var secret1 = Secret.FromSecretNameV2(this, "ExistingSecret", $"{environmentName}/ConfigTest/Example__Setting1");

        secret1.GrantRead(function);

        var secret2 = Secret.FromSecretNameV2(this, "TestingSecret", $"{environmentName}/ConfigTest/CDK_Secret");

        secret2.GrantRead(function);
        
        var api = new LambdaRestApi(this,
                                    "ConfigTestApiGateway",
                                    new LambdaRestApiProps
                                    {
                                        Handler = function,
                                        Proxy = true,
                                        DefaultMethodOptions = new MethodOptions { ApiKeyRequired = true }
                                    });

        var plan = api.AddUsagePlan("ConfigTestUsagePlan",
                                    new UsagePlanProps
                                    {
                                        Name = "ConfigTestUsagePlan",
                                        Throttle = new ThrottleSettings
                                                   {
                                                       BurstLimit = 200,
                                                       RateLimit = 100
                                                   },
                                        Quota = new QuotaSettings
                                                {
                                                    Limit = 5000,
                                                    Period = Period.MONTH
                                                }
                                    });

        plan.AddApiKey(api.AddApiKey("ConfigTestApiKey"));

        plan.AddApiStage(new UsagePlanPerApiStage
                         {
                             Api = api,
                             Stage = api.DeploymentStage
                         });

        var target = new ApiGateway(api,
                                    new ApiGatewayProps
                                    {
                                        Path = "/api/Values",
                                        Method = "GET"
                                    });

        new Rule(this,
                "ConfigTestEvent",
                new RuleProps
                {
                    Schedule = Schedule.Rate(Duration.Hours(12)),
                    Targets = new IRuleTarget[] { target }
                });
    }
}