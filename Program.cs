using Hardnessapi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HardnessDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("HardnessDb");
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
