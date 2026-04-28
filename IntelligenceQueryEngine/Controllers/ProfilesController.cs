using IntelligenceQueryEngine.Model.Dto;
using IntelligenceQueryEngine.Models;
using IntelligenceQueryEngine.Services;
using Microsoft.AspNetCore.Mvc;

namespace IntelligenceQueryEngine.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfilesController : ControllerBase
{
    private readonly ProfileService _service;
    private readonly NaturalLanguageParser _parser;

    public ProfilesController(ProfileService service) { _service = service; _parser = new(); }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? gender,
        [FromQuery] string? age_group,
        [FromQuery] string? country_id,
        [FromQuery] int? min_age,
        [FromQuery] int? max_age,
        [FromQuery] double? min_gender_probability,
        [FromQuery] double? min_country_probability,
        [FromQuery] string? sort_by = "created_at",
        [FromQuery] string? order = "asc",
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10)
    {
        var query = new QueryParams
        {
            gender = gender,
            age_group = age_group,
            country_id = country_id,
            min_age = min_age,
            max_age = max_age,
            min_gender_probability = min_gender_probability,
            min_country_probability = min_country_probability,
            sort_by = sort_by,
            order = order,
            page = page,
            limit = Math.Min(limit, 50)
        };

        var (profiles, total) = await _service.GetAsync(query);
        return Ok(new ApiResponse<List<Profile>> { Status = "success", Page = page, Limit = limit, Total = total, Data = profiles });
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? q,
        [FromQuery] string? sort_by = "created_at",
        [FromQuery] string? order = "asc",
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new ErrorResponse { Message = "Missing or empty query parameter" });

        var query = _parser.Parse(q);
        if (!_parser.CanInterpret(query))
            return UnprocessableEntity(new ErrorResponse { Message = "Unable to interpret query" });

        query.sort_by = sort_by;
        query.order = order;
        query.page = page;
        query.limit = Math.Min(limit, 50);

        var (profiles, total) = await _service.GetAsync(query);
        return Ok(new ApiResponse<List<Profile>> { Status = "success", Page = page, Limit = limit, Total = total, Data = profiles });
    }
}