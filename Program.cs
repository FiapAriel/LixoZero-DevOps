using LixoZero.Data;
using LixoZero.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1) Connection string: ENV -> appsettings -> fallback local
var cs = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
         ?? builder.Configuration.GetConnectionString("DefaultConnection")
         ?? "Data Source=lixoZero.db";

// 2) DI padrão
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

// 3) Garantir base (aplica Migrate; se falhar, EnsureCreated para evitar 500 por 'no such table/unable to open')
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

// 4) Pipeline / Swagger
// Habilita Swagger se em Development OU se ENABLE_SWAGGER=true (controlado pelo pipeline)
var enableSwaggerEnv = Environment.GetEnvironmentVariable("ENABLE_SWAGGER");
var enableSwagger = app.Environment.IsDevelopment() ||
                    string.Equals(enableSwaggerEnv, "true", StringComparison.OrdinalIgnoreCase);

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 5) HTTPS redirect: só ativa se NÃO estiver desabilitado
var disableHttps = Environment.GetEnvironmentVariable("DISABLE_HTTPS_REDIRECT");
if (!string.Equals(disableHttps, "true", StringComparison.OrdinalIgnoreCase))
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
