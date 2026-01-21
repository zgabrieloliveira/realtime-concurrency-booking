using Application.Dtos;
using Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[ApiController]
[Route("api/seats")]
public class SeatsController : ControllerBase
{
    private readonly HoldSeatUseCase _holdSeatUseCase;

    public SeatsController(HoldSeatUseCase holdSeatUseCase)
    {
        _holdSeatUseCase = holdSeatUseCase;
    }
    
    [HttpPost("hold")]
    public async Task<IActionResult> HoldSeat([FromBody] HoldSeatRequestDto request)
    {
        try
        {
            await _holdSeatUseCase.ExecuteAsync(request);
            return Ok(new { message = "Seat locked successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
}