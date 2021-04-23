# Welcome to your CDK C# project!

This is a blank project for C# development with CDK.

The `cdk.json` file tells the CDK Toolkit how to execute your app.

It uses the [.NET Core CLI](https://docs.microsoft.com/dotnet/articles/core/) to compile and execute your project.


## Useful commands

* `set AWS_PROFILE=ICS-Spikes_Admin` set AWS profile
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