Clean up the default template files from the Web API scaffold, then add the required NuGet packages and create the test project.

1. Delete these files from TradeBlotter_Interview/:
   - WeatherForecast.cs
   - Controllers/WeatherForecastController.cs

2. Add NuGet packages to TradeBlotter_Interview/TradeBlotter_Interview.csproj:
   dotnet add TradeBlotter_Interview package Microsoft.EntityFrameworkCore.Sqlite
   dotnet add TradeBlotter_Interview package Microsoft.EntityFrameworkCore.Design

3. Create a new xUnit test project:
   dotnet new xunit -n TradeBlotter_Interview.Tests
   dotnet sln add TradeBlotter_Interview.Tests
   dotnet add TradeBlotter_Interview.Tests reference TradeBlotter_Interview/TradeBlotter_Interview.csproj

4. Add to TradeBlotter_Interview.Tests:
   dotnet add TradeBlotter_Interview.Tests package Microsoft.EntityFrameworkCore.InMemory
   dotnet add TradeBlotter_Interview.Tests package Moq

Show me the final .csproj files for both projects and confirm dotnet build succeeds.
