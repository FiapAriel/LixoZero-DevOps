using LixoZero.Data;
using Microsoft.EntityFrameworkCore;
using LixoZero.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<DescarteService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=lixoZero.db"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var disableHttpsRedirect = Environment.GetEnvironmentVariable("DISABLE_HTTPS_REDIRECT");
var shouldRedirectHttps = !string.Equals(disableHttpsRedirect, "true", StringComparison.OrdinalIgnoreCase);
if (shouldRedirectHttps)
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
