using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using IAM = Amazon.CDK.AWS.IAM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApiGatewayResearch
{
    class RouteToS3Props
    {
        public RestApi Api { get; internal set; }
        public string PathInApiGateway { get; internal set; }
        public string S3Bucket { get; internal set; }
        public string S3Prefix { get; internal set; }
        public IAM.IRole Role { get; internal set; }
    }

    internal class RouteToS3 : Construct
    {
        public RouteToS3(Constructs.Construct scope, string id, RouteToS3Props props) : base(scope, id)
        {
            var method = props.Api.Root.AddResource(props.PathInApiGateway)
                .AddResource("{proxy+}")
                .AddMethod("GET", new AwsIntegration(new AwsIntegrationProps
                {
                    Service = "s3",
                    Path = string.Join("/", (new[] { props.S3Bucket, props.S3Prefix, "{proxy}" }).Where(x => !string.IsNullOrWhiteSpace(x))),
                    IntegrationHttpMethod = "GET",
                    Options = new IntegrationOptions
                    {
                        CredentialsRole = props.Role ?? CreateRole(),
                        RequestParameters = new Dictionary<string, string>
                        {
                            { "integration.request.path.proxy", "method.request.path.proxy" }
                        },
                        IntegrationResponses = new[]
                        {
                            new IntegrationResponse
                            {
                                StatusCode = "200",
                                ResponseParameters = new Dictionary<string, string>
                                {
                                    { "method.response.header.Cache-Control", "integration.response.header.Cache-Control" },
                                    { "method.response.header.ETag", "integration.response.header.ETag" },
                                    { "method.response.header.Last-Modified", "integration.response.header.Last-Modified" },
                                    { "method.response.header.Content-Type", "integration.response.header.Content-Type" }
                                }
                            }
                        }
                    }
                }), new MethodOptions
                {
                    RequestParameters = new Dictionary<string, bool>
                    {
                        { "method.request.path.proxy", true }
                    },
                    MethodResponses = new[]
                    {
                        new MethodResponse
                        {
                            StatusCode = "200",
                            ResponseParameters = new Dictionary<string, bool>
                            {
                                { "method.response.header.Cache-Control", true },
                                { "method.response.header.Content-Type", true },
                                { "method.response.header.ETag", true },
                                { "method.response.header.Last-Modified", true },
                            }
                        }
                    }
                });
        }

        private IAM.IRole CreateRole() =>
            new IAM.Role(this, "s3role", new IAM.RoleProps
            {
                AssumedBy = new IAM.ServicePrincipal("apigateway.amazonaws.com"),
                RoleName = "ApiReadS3",
                ManagedPolicies = new[]
                {
                    IAM.ManagedPolicy.FromAwsManagedPolicyName("AmazonS3ReadOnlyAccess")
                }
            });
    }
}
