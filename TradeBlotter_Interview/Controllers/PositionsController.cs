using Microsoft.AspNetCore.Mvc;
using TradeBlotter_Interview.Models;
using TradeBlotter_Interview.Services;

namespace TradeBlotter_Interview.Controllers;

[ApiController]
[Route("[controller]")]
public class PositionsController(ITradeService tradeService, IPositionService positionService, ILogger<PositionsController> logger) : ControllerBase
{
    // Derives current open positions from the full trade history. Nothing is
    // persisted here — positions are always recomputed from trades on demand.
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Position>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var trades = await tradeService.GetAllAsync();
            var positions = positionService.CalculatePositions(trades);
            return Ok(positions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error while calculating positions");
            return Problem("An error occurred while calculating positions.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
