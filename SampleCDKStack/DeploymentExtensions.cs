using Amazon.CDK.AWS.APIGateway;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiGatewayResearch
{
    public static class DeploymentExtensions
    {
        public static Stage AddStage(this Deployment deployment, string name) =>
            new Stage(deployment.Stack, name, new StageProps
            {
                Deployment = deployment,
                StageName = name,
                Variables = new Dictionary<string, string>
                {
                    { "lambdaAlias", name }
                }
            });
    }
}
