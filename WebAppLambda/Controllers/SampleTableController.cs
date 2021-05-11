using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        public async Task<List<ShipTestTable>> Get(bool delay = false)
        {
            var context = new DynamoDBContext(_dynamoDbClient);

            if (delay)
            {
                var recorder = new Amazon.XRay.Recorder.Core.AWSXRayRecorder();
                recorder.BeginSubsegment("Waiting 200ms");
                await Task.Delay(200);
                recorder.EndSubsegment();
            }

            var asyncSearch = context.ScanAsync<ShipTestTable>(Enumerable.Empty<ScanCondition>());

            var results = await asyncSearch.GetNextSetAsync();

            return results;
        }

        [HttpGet("partiql/{id}")]
        public async Task<List<ShipTestTable>> Get(string id)
        {
            var res = await _dynamoDbClient.ExecuteStatementAsync(new Amazon.DynamoDBv2.Model.ExecuteStatementRequest
            {
                Statement = "SELECT * FROM SampleTable WHERE PartitionKey = ?",
                Parameters = new List<Amazon.DynamoDBv2.Model.AttributeValue>
                {
                    new Amazon.DynamoDBv2.Model.AttributeValue
                    {
                        S = id
                    }
                }
            });

            return res.Items.ConvertAll(x =>
            {
                var doc = Amazon.DynamoDBv2.DocumentModel.Document.FromAttributeMap(x);
                return new ShipTestTable
                {
                    PartitionKey = doc[nameof(ShipTestTable.PartitionKey)],
                    SortKey = doc[nameof(ShipTestTable.SortKey)],
                };
            });
        }
    }

    [DynamoDBTable("SampleTable")]
    public class ShipTestTable
    {
        [DynamoDBHashKey("PartitionKey")]
        public string PartitionKey { get; set; }

        [DynamoDBRangeKey("SortKey")]
        public string SortKey { get; set; }
    }
}
