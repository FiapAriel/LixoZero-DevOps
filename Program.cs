using LixoZero.Data;
using Microsoft.EntityFrameworkCore;
using LixoZero.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<DescarteService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=/app/db/lixoZero.db"));

var app = builder.Build();

// Swagger em Dev
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

// cria tabelas se não existirem
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
