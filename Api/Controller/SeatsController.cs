using Application.Dtos;
using Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller;

[ApiController]
[Route("Api/Seats")]
public class SeatsController : ControllerBase
{
    private readonly GetSeatsUseCase _getSeatsUseCase;
    private readonly HoldSeatUseCase _holdSeatUseCase;
    private readonly BuySeatUseCase _buySeatUseCase;
    
    public SeatsController(GetSeatsUseCase getSeatsUseCase, HoldSeatUseCase holdSeatUseCase, BuySeatUseCase buySeatUseCase)
    {
        _getSeatsUseCase = getSeatsUseCase;
        _holdSeatUseCase = holdSeatUseCase;
        _buySeatUseCase = buySeatUseCase;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var seats = await _getSeatsUseCase.ExecuteAsync();
        return Ok(seats);
    }
    
    [HttpPost("Hold")]
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
    
    [HttpPost("Buy")]
    public async Task<IActionResult> BuySeat([FromBody] BuySeatRequestDto request)
    {
        try
        {
            await _buySeatUseCase.ExecuteAsync(request);
            return Ok(new { message = "Seat purchased successfully!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
}