using Amazon.CDK;
using ConfigTest.Deploy;

var app = new App();

new ConfigTestStack(app,
                    "ConfigTestStack",
                    new StackProps());

app.Synth();