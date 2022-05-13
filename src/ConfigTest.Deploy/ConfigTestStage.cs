using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.CDK;
using Constructs;

namespace ConfigTest.Deploy
{
    public class ConfigTestStage : Stage
    {
        public ConfigTestStage(Construct scope, string id, IStageProps? props = null) 
            : base(scope, id, props)
        {
            Stack stack = new ConfigTestStack(this, "ConfigTestStack");
        }
    }
}
