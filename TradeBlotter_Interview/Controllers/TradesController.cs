using Microsoft.AspNetCore.Mvc;
using TradeBlotter_Interview.DTOs;
using TradeBlotter_Interview.Services;

namespace TradeBlotter_Interview.Controllers;

[ApiController]
[Route("[controller]")]
public class TradesController(ITradeService tradeService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(TradeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTradeRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var trade = await tradeService.AddAsync(request);

        var response = new TradeResponse(
            trade.Id,
            trade.Symbol,
            trade.Side,
            trade.Quantity,
            trade.Price,
            trade.Timestamp);

        return CreatedAtAction(nameof(GetAll), null, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TradeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var trades = await tradeService.GetAllAsync();

        var response = trades.Select(t => new TradeResponse(
            t.Id,
            t.Symbol,
            t.Side,
            t.Quantity,
            t.Price,
            t.Timestamp));

        return Ok(response);
    }
}
