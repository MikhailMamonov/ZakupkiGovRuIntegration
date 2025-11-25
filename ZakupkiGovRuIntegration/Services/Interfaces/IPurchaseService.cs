using ZakupkiGovRuIntegration.Models;

namespace ZakupkiGovRuIntegration.Services.Interfaces
{
    public interface IPurchaseService
    {
        Task<Dictionary<string,Dictionary<string,string>>> Find(string regNumber);
    }
}
