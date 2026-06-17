In TradeBlotter_Interview/Data/TradeBlotterDbContext.cs:
- Create EF Core DbContext with DbSet<Trade> Trades
- Configure Trade entity: Symbol max length 20, Side max length 4, add index on Symbol

In TradeBlotter_Interview/Program.cs:
- Remove the WeatherForecast minimal API endpoint if still present
- Register TradeBlotterDbContext with SQLite: connection string "Data Source=trades.db"
- Call db.Database.EnsureCreated() on startup (get DbContext from app.Services)
- Add CORS policy named "AllowVue" allowing origin http://localhost:5173, all methods, all headers
- Use app.UseCors("AllowVue")
- Add controllers with JSON options: camelCase property names, enums serialized as strings

Show me the complete updated Program.cs.
