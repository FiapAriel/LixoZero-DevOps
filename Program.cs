using LixoZero.Data;
using Microsoft.EntityFrameworkCore;
using LixoZero.Services;

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
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3) Garantir DB/tabelas (evita 500 por "no such table/unable to open")
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

// 4) Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona para HTTPS apenas se NÃO estiver desabilitado por variável de ambiente
var disableHttps = Environment.GetEnvironmentVariable("DISABLE_HTTPS_REDIRECT");
if (!string.Equals(disableHttps, "true", StringComparison.OrdinalIgnoreCase))
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
