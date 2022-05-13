using Amazon.CDK;
using ConfigTest.Deploy;

var app = new App();

new ConfigTestPipelineStack(app, "ConfigTestPipelineStack",
                            new StackProps
                            {
                                Env = new Amazon.CDK.Environment
                                      {
                                          Account = "593374787003",
                                          Region = "us-east-1"
                                      }
                            });
//new ConfigTestStack(app,
//                    "ConfigTestStack",
//                    new StackProps());

app.Synth();