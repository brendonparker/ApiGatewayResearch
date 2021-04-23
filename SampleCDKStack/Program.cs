using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiGatewayResearch
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new ApiGatewayCDKStack(app, "ApiGatewayCDKStack");
            app.Synth();
        }
    }
}
