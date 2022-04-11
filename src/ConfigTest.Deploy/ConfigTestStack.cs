using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;
using Stack = Amazon.CDK.Stack;

namespace ConfigTest.Deploy;

public class ConfigTestStack : Stack
{
    public ConfigTestStack(Construct scope, string id, IStackProps props)
        : base(scope, id, props)
    {
        var table = new Table(this,
                              "ExampleTable",
                              new TableProps
                              {
                                  BillingMode = BillingMode.PAY_PER_REQUEST,
                                  Encryption = TableEncryption.DEFAULT,
                                  PartitionKey =
                                      new Attribute { Name = "Product", Type = AttributeType.STRING },
                                  SortKey = new Attribute
                                            {
                                                Name = "ApplicationPageUrl",
                                                Type = AttributeType.STRING
                                            },
                                  TableClass = TableClass.STANDARD,
                                  TableName = "ExampleTable"
                              });

        var environmentVariables = EnvironmentVariablesFactory.AddEnvironmentVariables(this);
        environmentVariables.Add("Example__Setting2", "From CDK");

        var function =
            new DockerImageFunction(this,
                                    "ConfigTestLambdas",
                                    new DockerImageFunctionProps
                                    {
                                        Code =
                                            DockerImageCode.FromImageAsset(@"src/ConfigTest"),
                                        Description = ".Net 6 Lambda to test reading config values",
                                        Environment = environmentVariables
                                    });

        table.GrantReadWriteData(function);

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

        var key = api.AddApiKey("ConfigTestApiKey");

        plan.AddApiKey(key);

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

        var rule = new Rule(this,
                            "ConfigTestEvent",
                            new RuleProps
                            {
                                Schedule = Schedule.Rate(Duration.Hours(12)),
                                Targets = new IRuleTarget[] { target }
                            });
    }
}