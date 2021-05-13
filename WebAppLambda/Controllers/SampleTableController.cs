
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppLambda.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpPost]
        public async Task<SampleTableEntry> Post(SampleTableEntry entry)
        {
            var res = await _sampleTableRepo.SaveAsync(entry);
            return res;
        }

        [HttpDelete]
        public async Task Delete(SampleTableEntry entry)
        {
            await _sampleTableRepo.DeleteAsync(entry.PartitionKey, entry.SortKey);
        }
    }
}