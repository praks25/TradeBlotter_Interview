using Microsoft.AspNetCore.Mvc;
using TradeBlotter_Interview.Models;
using TradeBlotter_Interview.Services;

namespace TradeBlotter_Interview.Controllers;

[ApiController]
[Route("[controller]")]
public class PositionsController(ITradeService tradeService, IPositionService positionService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Position>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var trades = await tradeService.GetAllAsync();
        var positions = positionService.CalculatePositions(trades);
        return Ok(positions);
    }
}
