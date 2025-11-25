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
            string url = $"";
            var response = await _client.GetAsync($"/epz/order/notice/ea20/view/common-info.html?regNumber={regNumber}");

            return await response.ReadContentAsync();
        }
    }
}
