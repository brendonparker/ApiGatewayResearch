using Microsoft.AspNetCore.Hosting;
using System;
using Serilog;
using System.Collections.Generic;
using Serilog.Formatting.Elasticsearch;

namespace WebAppLambda
{
    public class LambdaEntryPoint : Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    {
        /// <summary>
        /// The builder has configuration, logging and Amazon API Gateway already configured. The startup class
        /// needs to be configured in this method using the UseStartup<>() method.
        /// </summary>
        /// <param name="builder"></param>
        protected override void Init(IWebHostBuilder builder)
        {
            builder
                .UseSerilog((ctx, config) =>
                {
                    config
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .WriteTo.Console(new ElasticsearchJsonFormatter());
                })
                .UseStartup<Startup>();
        }
    }
}
