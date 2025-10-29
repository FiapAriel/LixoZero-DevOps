using LixoZero.Data;
using LixoZero.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1) Connection string: ENV -> appsettings -> fallback local
var cs = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
         ?? builder.Configuration.GetConnectionString("DefaultConnection")
         ?? "Data Source=lixoZero.db";

// 2) DI
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(cs));
builder.Services.AddScoped<DescarteService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LixoZero API",
        Version = "v1",
        Description = "API do LixoZero"
    });
});

var app = builder.Build();

// 3) DB: aplica migrações (fallback EnsureCreated para não quebrar em runtime)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DB] Migrate falhou: {ex.Message}. Tentando EnsureCreated()...");
        db.Database.EnsureCreated();
    }
}

// 4) Swagger: ON em Development e Staging; também aceita ENABLE_SWAGGER=true
var enableSwaggerEnv = Environment.GetEnvironmentVariable("ENABLE_SWAGGER");
var enableSwagger =
    app.Environment.IsDevelopment() ||
    app.Environment.IsStaging() ||
    string.Equals(enableSwaggerEnv, "true", StringComparison.OrdinalIgnoreCase);

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Fallback quando Swagger estiver OFF: garante 200 para o healthcheck
    var methods = new[] { "GET", "HEAD" };
    app.MapMethods("/swagger/index.html", methods, () => Results.Text("Swagger desabilitado neste ambiente.", "text/plain"));
}

// 5) HTTPS redirect somente se HÁ HTTPS configurado (evita 307 em http://localhost:5038)
var disableHttps = Environment.GetEnvironmentVariable("DISABLE_HTTPS_REDIRECT");
var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")
           ?? Environment.GetEnvironmentVariable("URLS")
           ?? string.Empty;
var httpsPorts = Environment.GetEnvironmentVariable("HTTPS_PORTS") ?? string.Empty;
bool hasHttpsBinding = urls.Contains("https://", StringComparison.OrdinalIgnoreCase)
                       || !string.IsNullOrWhiteSpace(httpsPorts);

if (!string.Equals(disableHttps, "true", StringComparison.OrdinalIgnoreCase) && hasHttpsBinding)
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
