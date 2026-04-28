using IntelligenceQueryEngine.Models;
using IntelligenceQueryEngine.Services.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntelligenceQueryEngine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {

        private readonly IExportService _exportService;
        private readonly IProfileService _profileService;

        public ExportController(IExportService exportService, IProfileService profileService)
        {
            _exportService = exportService;
            _profileService = profileService;
        }

        [HttpPost("csv")]
        public async Task<IActionResult> ExportToCsv([FromBody] QueryParams query)
        {
            // Validate limit for export (max 10,000 records)
            query.limit = Math.Min(query.limit > 0 ? query.limit : 1000, 10000);
            query.page = 1;

            var csvData = await _exportService.ExportProfilesToCsvAsync(query);
            var fileName = _exportService.GetCsvFileName(query);

            return File(csvData, "text/csv", fileName);
        }

        [HttpGet("csv")]
        public async Task<IActionResult> ExportToCsvGet(
       [FromQuery] string? gender,
       [FromQuery] string? age_group,
       [FromQuery] string? country_id,
       [FromQuery] int? min_age,
       [FromQuery] int? max_age)
        {
            var query = new QueryParams
            {
                gender = gender,
                age_group = age_group,
                country_id = country_id,
                min_age = min_age,
                max_age = max_age,
                limit = 10000,
                page = 1
            };

            var csvData = await _exportService.ExportProfilesToCsvAsync(query);
            var fileName = _exportService.GetCsvFileName(query);

            return File(csvData, "text/csv", fileName);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetExportCount(
       [FromQuery] string? gender,
       [FromQuery] string? age_group,
       [FromQuery] string? country_id,
       [FromQuery] int? min_age,
       [FromQuery] int? max_age)
        {
            var query = new QueryParams
            {
                gender = gender,
                age_group = age_group,
                country_id = country_id,
                min_age = min_age,
                max_age = max_age,
                limit = 1,
                page = 1
            };

            var (_, total) = await _profileService.GetProfilesAsync(query);
            return Ok(new { total_records = total, max_export = 10000 });
        }
    }
}
