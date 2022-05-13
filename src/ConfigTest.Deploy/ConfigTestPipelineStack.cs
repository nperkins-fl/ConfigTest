using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.CDK;
using Amazon.CDK.Pipelines;
using Constructs;

namespace ConfigTest.Deploy
{
    public class ConfigTestPipelineStack : Stack
    {
        public ConfigTestPipelineStack(Construct scope, string id, IStackProps? props = null) 
            : base(scope, id, props)
        {
            var pipeline = new CodePipeline(this,
                                            "pipeline",
                                            new CodePipelineProps
                                            {
                                                PipelineName = "ConfigTestPipeline",
                                                CrossAccountKeys = true,
                                                Synth = new ShellStep("Synth",
                                                                      new ShellStepProps
                                                                      {
                                                                          Input =
                                                                              CodePipelineSource
                                                                                  .GitHub("nperkins-fl/ConfigTest",
                                                                                      "pipeline"),
                                                                          Commands = new string[]
                                                                              { "/usr/local/bin/dotnet-install.sh --channel LTS", "npm install -g aws-cdk", "cdk synth" }
                                                                      })
                                            });

            pipeline.AddStage(new ConfigTestStage(this,
                                                  "test",
                                                  new StageProps
                                                  {
                                                      Env = new Amazon.CDK.Environment
                                                            {
                                                                Account = "593374787003",
                                                                Region = "us-east-1"
                                                            }
                                                  }));
        }
    }
}
