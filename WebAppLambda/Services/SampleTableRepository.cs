using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppLambda
{
    public interface ISampleTableRepository
    {
        Task<List<SampleTableEntry>> GetAllAsync();
        Task<List<SampleTableEntry>> GetSingleAsync(string id);
        Task<SampleTableEntry> SaveAsync(SampleTableEntry sampleTableEntry);
        Task DeleteAsync(string partitionKey, string sortKey);
    }

    public class SampleTableRepository : ISampleTableRepository
    {
        private ILogger<SampleTableRepository> _logger;
        private IAmazonDynamoDB _dynamoDbClient;

        public SampleTableRepository(
            ILogger<SampleTableRepository> logger,
            IAmazonDynamoDB dynamoDbClient)
        {
            _logger = logger;
            _dynamoDbClient = dynamoDbClient;
        }

        public async Task<List<SampleTableEntry>> GetAllAsync()
        {
            var res = await _dynamoDbClient.ExecuteStatementAsync(new ExecuteStatementRequest
            {
                Statement = "SELECT * FROM SampleTable"
            });

            return res.Items.ConvertAll(Convert);
        }

        public async Task<List<SampleTableEntry>> GetSingleAsync(string id)
        {
            var res = await _dynamoDbClient.ExecuteStatementAsync(new ExecuteStatementRequest
            {
                Statement = "SELECT * FROM SampleTable WHERE PartitionKey = ?",
                Parameters = new List<AttributeValue>
                {
                    new AttributeValue
                    {
                        S = id
                    }
                }
            });

            return res.Items.ConvertAll(Convert);
        }

        public SampleTableEntry Convert(Dictionary<string, AttributeValue> source)
        {
            var doc = Amazon.DynamoDBv2.DocumentModel.Document.FromAttributeMap(source);
            return new SampleTableEntry
            {
                PartitionKey = doc[nameof(SampleTableEntry.PartitionKey)],
                SortKey = doc[nameof(SampleTableEntry.SortKey)],
            };
        }

        public async Task<SampleTableEntry> SaveAsync(SampleTableEntry sampleTableEntry)
        {
            var context = new Amazon.DynamoDBv2.DataModel.DynamoDBContext(_dynamoDbClient);

            await context.SaveAsync(new SampleTable
            {
                PartitionKey = sampleTableEntry.PartitionKey,
                SortKey = sampleTableEntry.SortKey
            });

            return sampleTableEntry;
        }

        public async Task DeleteAsync(string partitionKey, string sortKey)
        {
            var res = await _dynamoDbClient.ExecuteStatementAsync(new ExecuteStatementRequest
            {
                Statement = "DELETE FROM SampleTable WHERE PartitionKey = ? AND SortKey = ?",
                Parameters = new List<AttributeValue>
                {
                    new AttributeValue
                    {
                        S = partitionKey
                    },
                    new AttributeValue
                    {
                        S = sortKey
                    }
                }
            });
        }

        [DynamoDBTable("SampleTable")]
        class SampleTable
        {
            [DynamoDBHashKey("PartitionKey")]
            public string PartitionKey { get; set; }
            [DynamoDBRangeKey("SortKey")]
            public string SortKey { get; set; }
        }
    }
}
