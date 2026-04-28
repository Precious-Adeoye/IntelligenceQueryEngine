using IntelligenceQueryEngine.Models;

namespace IntelligenceQueryEngine.Services.Contract
{
    public interface IExportService
    {
        Task<byte[]> ExportProfilesToCsvAsync(QueryParams query);
        string GetCsvFileName(QueryParams query);
    }
}
