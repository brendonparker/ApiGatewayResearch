# Sample dotnet core app using API Gateway and Lambdas

It uses the [.NET Core CLI](https://docs.microsoft.com/dotnet/articles/core/) to compile and execute your project.

## Deployment

First, run: `dotnet publish WebAppLambda/WebAppLambda.csproj -o ./LambdaSource/` to create the source artifacts to be used by the lambda function.

Then, run: `cdk deploy` to deploy the stack. You may need to add the `--profile` option if you need to use a profile other than default.

## Useful commands

`dotnet publish WebAppLambda/WebAppLambda.csproj -o ./LambdaSource/`

* `dotnet build src` compile this app
* `cdk deploy`       deploy this stack to your default AWS account/region
* `cdk diff`         compare deployed stack with current state
* `cdk synth`        emits the synthesized CloudFormation template

## AWS Lambda Power Tuning

Useful tool for benchmarking lambda performance:
https://github.com/alexcasalboni/aws-lambda-power-tuning

See SampleApiGatwayPayload.json for sample state machine input

Runs:
[Basic ASPNET App](https://lambda-power-tuning.show/#gAAAAQACAAQABgAI;47QERRYsg0SRVPVDwDFmQ0PSHUOHlghD;i6qVNp35kzZUZIo20TeCNq+ZhTYwdZo2)
[ASPNET App with DynamoDB call (cold starts)](https://lambda-power-tuning.show/#gAAAAQACAAQABgAI;meUJRoAohEUmOABF3Zx4RLuaI0TJ5wxE;wHqbNzIIlTe/l5A3VjmMN112ijet9543)
[ASPNET App with DynamoDB call (warm starts)](https://lambda-power-tuning.show/#gAAAAQACAAQABgAI;y2FRQ3wNVUMywdVBmG6TQc3MakFVVZBB;lMLsNBFFcTVPhnM0hF6rNO3vyjSEXis1)