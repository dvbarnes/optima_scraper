using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using optima_tracking.Data;

namespace optima_tracking
{
    public class TimerScraper
    {
        private readonly IUnitDatastore _dataStore;
        private readonly Scraper _scraper;
#if DEBUG
        private const string TIME = "0 */1 * * * *";
#elif RELEASE
        private const string TIME = "* * */12 * * *";
#endif
        public TimerScraper(IUnitDatastore dataStore, Scraper scraper)
        {
            _dataStore = dataStore;
            _scraper = scraper;
        }

        [FunctionName("TimerScraper")]
        public async Task Run([TimerTrigger(TIME)]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var data = await _scraper.ScrapeAsync();
            foreach (var unit in data)
            {
                await _dataStore.SaveAsync(unit);
            }
        }
    }
}
