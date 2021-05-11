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
        private readonly ISampleTableRepository _sampleTableRepo;

        public SampleTableController(
            ILogger<SampleTableController> logger,
            ISampleTableRepository sampleTableRepo)
        {
            _logger = logger;
            _sampleTableRepo = sampleTableRepo;
        }

        [HttpGet]
        public async Task<List<SampleTableEntry>> Get(bool delay = false)
        {
            var res = await _sampleTableRepo.GetAllAsync();
            if (delay)
            {
                var recorder = new Amazon.XRay.Recorder.Core.AWSXRayRecorder();
                recorder.BeginSubsegment("Waiting 200ms");
                await Task.Delay(200);
                recorder.EndSubsegment();
            }

            return res;
        }

        //[HttpGet("partiql/{id}")]
        //public async Task<SampleTableEntry> Get(string id)
        //{
        //    var res = await _dynamoDbClient.ExecuteStatementAsync(new Amazon.DynamoDBv2.Model.ExecuteStatementRequest
        //    {
        //        Statement = "SELECT * FROM SampleTable WHERE PartitionKey = ?",
        //        Parameters = new List<Amazon.DynamoDBv2.Model.AttributeValue>
        //        {
        //            new Amazon.DynamoDBv2.Model.AttributeValue
        //            {
        //                S = id
        //            }
        //        }
        //    });

        //    return res.Items.ConvertAll(x =>
        //    {
        //        var doc = Amazon.DynamoDBv2.DocumentModel.Document.FromAttributeMap(x);
        //        return new ShipTestTable
        //        {
        //            PartitionKey = doc[nameof(ShipTestTable.PartitionKey)],
        //            SortKey = doc[nameof(ShipTestTable.SortKey)],
        //        };
        //    });
        //}
    }
}
