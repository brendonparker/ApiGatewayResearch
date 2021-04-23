using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using IAM = Amazon.CDK.AWS.IAM;
using Lambda = Amazon.CDK.AWS.Lambda;
using System.Collections.Generic;

namespace ApiGatewayResearch
{
    public class ApiGatewayCDKStack : Stack
    {
        internal ApiGatewayCDKStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var api = new RestApi(this, "MyApi2", new RestApiProps
            {
                RestApiName = "my-api2",
                Deploy = false
            });

            var deployment = new Deployment(this, "preprd", new DeploymentProps
            {
                Api = api,
                Description = "Initial Deployment"
            });

            deployment.AddStage("prod");
            deployment.AddStage("preprod");

            var lambda = new Lambda.Function(this, "MyLambda", new Lambda.FunctionProps
            {
                FunctionName = "DemoProxyLambda",
                Code = Lambda.Code.FromAsset("./SampleCDKStack/lambda"),
                Handler = "index.handler",
                Runtime = Lambda.Runtime.NODEJS_14_X,
                CurrentVersionOptions = new Lambda.VersionOptions
                {
                    RemovalPolicy = RemovalPolicy.RETAIN,
                    RetryAttempts = 1,
                },
                Environment = new Dictionary<string, string>
                {
                    {  "CodeVersionString", "0.0.8" }
                }
            });

            lambda.CurrentVersion.AddAlias("preprod");

            api.Root.AddProxy(new ProxyResourceOptions
            {
                DefaultIntegration = StageSpecificLambda(lambda)
            });
        }

        // Hackiness to set stagevariable: https://github.com/aws/aws-cdk/issues/6143#issuecomment-720432475
        private AwsIntegration StageSpecificLambda(Lambda.Function lambda)
        {
            var credentialsRole = new IAM.Role(this, "apigateway-api-role", new IAM.RoleProps
            {
                AssumedBy = new IAM.ServicePrincipal("apigateway.amazonaws.com"),
            });

            // Add the regular lambda Arns to the credentialsRole
            credentialsRole.AddToPolicy(
                new IAM.PolicyStatement(new IAM.PolicyStatementProps
                {
                    Actions = new[] { "lambda:InvokeFunction" },
                    Resources = new[] 
                    { 
                        lambda.FunctionArn, 
                        $"{lambda.FunctionArn}:*" 
                    },
                    Effect = IAM.Effect.ALLOW
                })
            );

            var stageLambda = Lambda.Function.FromFunctionArn(this, "MyLambda-preprod", lambda.FunctionArn + ":${stageVariables.lambdaAlias}");

            // Add the stageLambda Arn to the integration. 
            var integration = new AwsIntegration(new AwsIntegrationProps
            {
                Proxy = true,
                Service = "lambda",
                Path = $"2015-03-31/functions/{stageLambda.FunctionArn}/invocations",
                Options = new IntegrationOptions
                {
                    CredentialsRole = credentialsRole,
                }
            });
            return integration;
        }
    }
}
