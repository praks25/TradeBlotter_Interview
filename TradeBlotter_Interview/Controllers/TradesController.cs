using Microsoft.AspNetCore.Mvc;
using TradeBlotter_Interview.DTOs;
using TradeBlotter_Interview.Services;

namespace TradeBlotter_Interview.Controllers;

[ApiController]
[Route("[controller]")]
public class TradesController(ITradeService tradeService, ILogger<TradesController> logger) : ControllerBase
{
    // Submits a new trade. Validation failures return 400 with field-level details;
    // unexpected failures (db, etc.) are logged and surfaced as a 500 ProblemDetails.
    [HttpPost]
    [ProducesResponseType(typeof(TradeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTradeRequest request)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning(
                "Trade submission failed validation: {Errors}",
                string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return ValidationProblem(ModelState);
        }

        try
        {
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
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error while creating trade for {Symbol}", request.Symbol);
            return Problem("An error occurred while submitting the trade.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    // Returns all trades, newest first.
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TradeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
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
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error while fetching trades");
            return Problem("An error occurred while fetching trades.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
