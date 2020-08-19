using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using optima_tracking.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace optima_tracking
{
    public class Scraper
    {
        public async Task<IEnumerable<UnitData>> ScrapeAsync(CancellationToken token = default)
        {
            var portalKeyUrl = @"https://optimasignature.securecafe.com/onlineleasing/optima-signature/oleapplication.aspx?stepname=Apartments&myOlePropertyId=565074";
            var url = @"https://optimasignature.securecafe.com/onlineleasing/rcLoadContent.ashx?contentclass=oleapplication&stepname=Apartments&t=0.46565860910689794&control=1";
            var web = new HtmlWeb();
            var getPage = web.Load(portalKeyUrl);
            var portalKey = getPage.GetElementbyId("cafeportalkey").GetAttributeValue("value", "");
            var client = new HttpClient();
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("myolePropertyID", "565074"),
                new KeyValuePair<string, string>("UnitCode", "01"),
                new KeyValuePair<string, string>("cafeportalkey", portalKey)
            });
            var html = await client.PostAsync(url, formContent);
            var htmlContent = await html.Content.ReadAsStringAsync();

            var doc2 = new HtmlDocument();
            doc2.LoadHtml(htmlContent);
            var nodes = doc2.DocumentNode.SelectNodes("//tr[contains(@class, 'AvailUnitRow')]");
            var date = DateTime.UtcNow;
            var list = new List<UnitData>();
            foreach (var node in nodes)
            {
                var caption = node.ParentNode.ParentNode.SelectSingleNode(".//caption").InnerText;
                var m = Regex.Match(caption, @"(\d) Bedrooms?, (\d) Bathrooms?");
                var aptNumber = int.Parse(node.SelectSingleNode(".//td[@data-label='Apartment']").InnerText.Trim('#'));
                var rooms = int.Parse(m.Groups[1].Value);
                var baths = int.Parse(m.Groups[2].Value);
                var sqlFt = int.Parse(node.SelectSingleNode(".//td[@data-label='Sq. Ft.']").InnerText);
                var rent = int.Parse(node.SelectSingleNode(".//td[@data-label='Rent']").InnerText.Trim('$').Replace(",", string.Empty));
                var dateAvailableStr = node.SelectSingleNode(".//td[@data-label='Date Available']").InnerText.Trim('$').Replace(",", string.Empty);
                var dateAvailable = DateTime.UtcNow;
                if (dateAvailableStr != "Available")
                {
                    dateAvailable = DateTime.Parse(dateAvailableStr);
                }
                list.Add(new UnitData
                {
                    DateRecorded = date,
                    ApartmentNumber = aptNumber,
                    Rooms = rooms,
                    Baths = baths,
                    DateAvailable = dateAvailable,
                    Rent = rent,
                    SquareFootage = sqlFt
                });
            }
            return list;
        }
    }
}
