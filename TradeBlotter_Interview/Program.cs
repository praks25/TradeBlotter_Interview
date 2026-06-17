using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TradeBlotter_Interview.Data;
using TradeBlotter_Interview.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TradeBlotterDbContext>(options =>
    options.UseSqlite("Data Source=trades.db"));

builder.Services.AddCors(options =>
    options.AddPolicy("AllowVue", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddScoped<ITradeService, TradeService>();
builder.Services.AddScoped<IPositionService, PositionService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TradeBlotterDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowVue");
app.UseAuthorization();
app.MapControllers();

app.Run();
