using Microsoft.AspNetCore.Mvc;
using PromoEngine.DTOs;
using PromoEngine.Services;

namespace PromoEngine.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmissionController : ControllerBase
{
    private readonly SubmissionService _submissionService;

    public SubmissionController(SubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] SubmissionRequestDto dto)
    {
        var result = await _submissionService.ProcessSubmissionAsync(dto);

        if (!result.Success)
        { 
            return BadRequest(result);
        }

        return Ok(result);
    }
}
