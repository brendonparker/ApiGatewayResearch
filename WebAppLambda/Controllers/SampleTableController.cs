using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppLambda.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleTableController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IAmazonDynamoDB _dynamoDbClient;

        public SampleTableController(
            ILogger<SampleTableController> logger,
            IAmazonDynamoDB dynamoDbClient)
        {
            _logger = logger;
            _dynamoDbClient = dynamoDbClient;
        }

        [HttpGet]
        public async Task<List<ShipTestTable>> Get()
        {
            var context = new DynamoDBContext(_dynamoDbClient);
            var recorder = new Amazon.XRay.Recorder.Core.AWSXRayRecorder();
            recorder.BeginSubsegment("Waiting 200ms");
            await Task.Delay(200);
            recorder.EndSubsegment();

            var asyncSearch = context.ScanAsync<ShipTestTable>(Enumerable.Empty<ScanCondition>());

            var results = await asyncSearch.GetNextSetAsync();

            return results;
        }
    }

    [DynamoDBTable("SampleTable")]
    public class ShipTestTable
    {
        [DynamoDBHashKey("PartitionKey")]
        public string ParitionKey { get; set; }

        [DynamoDBRangeKey("SortKey")]
        public string SortKey { get; set; }
    }
}
