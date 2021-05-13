using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using IAM = Amazon.CDK.AWS.IAM;
using Lambda = Amazon.CDK.AWS.Lambda;
using DynamoDB = Amazon.CDK.AWS.DynamoDB;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace ApiGatewayResearch
{
    public class ApiGatewayCDKStack : Stack
    {
        internal ApiGatewayCDKStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var api = new RestApi(this, "MyApi2", new RestApiProps
            {
                RestApiName = "my-api2",
                Deploy = true,
                DeployOptions = new StageOptions
                {
                    StageName = "preprod",
                    Variables = new Dictionary<string, string>
                    {
                        { "lambdaAlias", "preprod" }
                    },
                    TracingEnabled = true
                }
            });

            api.LatestDeployment.AddStage("prod");

            //var deployment = new Deployment(this, "preprd", new DeploymentProps
            //{
            //    Api = api,
            //    Description = "Initial Deployment",
            //    RetainDeployments
            //});

            //deployment.AddStage("prod");
            //deployment.AddStage("preprod");

            var ddbTable = new DynamoDB.Table(this, "SampleTable", new DynamoDB.TableProps
            {
                TableName = "SampleTable",
                PartitionKey = new DynamoDB.Attribute
                {
                    Name = "PartitionKey",
                    Type = DynamoDB.AttributeType.STRING
                },
                SortKey = new DynamoDB.Attribute
                {
                    Name = "SortKey",
                    Type = DynamoDB.AttributeType.STRING
                }
            });

            var lambda = new Lambda.Function(this, "MyLambda", new Lambda.FunctionProps
            {
                FunctionName = "DemoProxyLambda",
                Code = Lambda.Code.FromAsset("./LambdaSource"),
                Handler = "WebAppLambda::WebAppLambda.LambdaEntryPoint::FunctionHandlerAsync",
                Runtime = Lambda.Runtime.DOTNET_CORE_3_1,
                MemorySize = 1536,
                Timeout = Duration.Seconds(30),
                CurrentVersionOptions = new Lambda.VersionOptions
                {
                    RemovalPolicy = RemovalPolicy.RETAIN,
                    RetryAttempts = 1,
                },
                Tracing = Lambda.Tracing.ACTIVE,
                Environment = new Dictionary<string, string>
                {
                    {  "CodeVersionString", "0.0.13" }
                }
            });

            lambda.Role.AttachInlinePolicy(new IAM.Policy(this, "lambdaiam", new IAM.PolicyProps
            {
                PolicyName = "DynamoDbAccess",
                Document = new IAM.PolicyDocument(new IAM.PolicyDocumentProps
                {
                    Statements = new[]
                    {
                        new IAM.PolicyStatement(new IAM.PolicyStatementProps
                        {
                            Effect = IAM.Effect.ALLOW,
                            Actions = new []
                            {
                                "dynamodb:BatchGetItem",
                                "dynamodb:BatchWriteItem",
                                "dynamodb:PutItem",
                                "dynamodb:DeleteItem",
                                "dynamodb:PartiQLUpdate",
                                "dynamodb:Scan",
                                "dynamodb:ListTagsOfResource",
                                "dynamodb:Query",
                                "dynamodb:UpdateItem",
                                "dynamodb:PartiQLSelect",
                                "dynamodb:DescribeTable",
                                "dynamodb:PartiQLInsert",
                                "dynamodb:GetItem",
                                "dynamodb:GetRecords",
                                "dynamodb:PartiQLDelete"
                            },
                            Resources = new []
                            {
                                ddbTable.TableArn
                            }
                        })
                    }
                })
            }));

            lambda.CurrentVersion.AddAlias("preprod");

            var awsIntegration = StageSpecificLambda(lambda);

            api.Root.AddProxy(new ProxyResourceOptions
            {
                DefaultIntegration = awsIntegration
            });

            api.Root.AddMethod("GET", awsIntegration);

            new RouteToS3(this, "s3route", new RouteToS3Props
            {
                Api = api,
                PathInApiGateway = "content",
                S3Bucket = "test-rfsship-content",
                S3Prefix = "content",
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
