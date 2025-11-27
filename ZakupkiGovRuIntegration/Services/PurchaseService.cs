using System.Text.Json;
using System.Text.Json.Nodes;

using ZakupkiGovRuIntegration.Helpers;
using ZakupkiGovRuIntegration.Models;
using ZakupkiGovRuIntegration.Services.Interfaces;

namespace ZakupkiGovRuIntegration.Services
{
    public class PurchaseService: IPurchaseService
    {
        private readonly HttpClient _client;

        public PurchaseService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<Dictionary<string,Dictionary<string,string>>> Find(string regNumber)
        {
            HttpResponseMessage response;
            if (regNumber.Length == 19)
            {
                response = await _client.GetAsync($"/epz/order/notice/ea20/view/common-info.html?regNumber={regNumber}");

                return await response.ReadContent44FZAsync();
            }
            else {
                response = await _client.GetAsync($"/api/mobile/proxy/917/223/purchase/public/purchase/info/common-info.html?regNumber={regNumber}");

                if (response.IsSuccessStatusCode == false)
                    throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");
                var dataAsString = await response.Content.ReadAsStringAsync();

                // Create a JsonNode DOM from a JSON string.
                JsonNode forecastNode = JsonNode.Parse(dataAsString)!;
                var options = new JsonSerializerOptions { WriteIndented = true };
                Console.WriteLine(forecastNode!.ToJsonString(options));
                var noticeId = (int)forecastNode["data"]!["noticeInfo"]!["id"]!;

                response = await _client.GetAsync($"/epz/order/notice/notice223/common-info.html?noticeInfoId={noticeId}");
                return await response.ReadContent223FZAsync();
            } 
        }
    }
}
