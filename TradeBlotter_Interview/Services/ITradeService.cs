using TradeBlotter_Interview.DTOs;
using TradeBlotter_Interview.Models;

namespace TradeBlotter_Interview.Services;

public interface ITradeService
{
    Task<IEnumerable<Trade>> GetAllAsync();
    Task<Trade> AddAsync(CreateTradeRequest request);
}
