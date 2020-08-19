using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using optima_tracking.Data;
using CsvHelper;
using System.Globalization;

namespace optima_tracking
{
    public class CSVExport
    {
        private readonly IUnitDatastore _unitDataStore;

        public CSVExport(IUnitDatastore unitDataStore)
        {
            _unitDataStore = unitDataStore;
        }
        [FunctionName("CSVExport")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var data = await _unitDataStore.ReadAllDataAsync();
            var sw = new StringWriter();
            using (var csv = new CsvWriter(sw, CultureInfo.InvariantCulture))
            {
                await csv.WriteRecordsAsync(data);
            }
            return new OkObjectResult(sw.ToString());
        }
    }
}
