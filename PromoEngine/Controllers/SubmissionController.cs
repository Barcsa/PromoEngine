using Microsoft.AspNetCore.Mvc;
using PromoEngine.DTOs;
using PromoEngine.Services;
using PromoEngine.Services.Interfaces;

namespace PromoEngine.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmissionController : ControllerBase
{
    private readonly ISubmissionService _submissionService;

    public SubmissionController(ISubmissionService submissionService)
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
