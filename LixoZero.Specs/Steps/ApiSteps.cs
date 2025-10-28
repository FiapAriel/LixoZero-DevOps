using System;
using System.Globalization;
using System.Net;
using System.Text.Json;
using FluentAssertions;
using LixoZero.Specs.Support;
using TechTalk.SpecFlow;

namespace LixoZero.Specs.Steps;

[Binding]
public class ApiSteps
{
    [Given(@"que a base URL da API é ""(.*)""")]
    public void GivenBaseUrl(string url)
    {
        var candidate = url?.Trim();

        // Trata vazio/placeholder e lixo de config
        if (string.IsNullOrWhiteSpace(candidate) ||
            candidate.Contains("BASE_URL", StringComparison.OrdinalIgnoreCase) ||
            candidate.Contains("<") || candidate.Contains(">"))
        {
            candidate = Environment.GetEnvironmentVariable("LIXOZERO_BASE_URL")?.Trim()
                        ?? World.BaseUrl
                        ?? "http://localhost:5038";
        }

        candidate = NormalizeBaseUrl(candidate);
        World.BaseUrl = candidate;
        World.Client.SetBaseUrl(candidate);
    }

    private static string NormalizeBaseUrl(string baseUrl)
    {
        var b = baseUrl.Trim();
        if (!b.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !b.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            b = "http://" + b.TrimStart('/');
        }
        return b.TrimEnd('/');
    }

    [When(@"eu fizer POST para ""(.*)"" com o corpo JSON:")]
    public async Task WhenPostWithBody(string path, string body)
    {
        World.LastResponse = await World.Client.PostRaw(path, body);

        if (World.LastResponse.StatusCode == HttpStatusCode.Created)
        {
            if (!string.IsNullOrWhiteSpace(World.LastResponse.Content))
            {
                using var doc = JsonDocument.Parse(World.LastResponse.Content!);
                World.CreatedId = TryExtractId(doc.RootElement) ?? World.CreatedId;
            }

            if (World.CreatedId is null)
            {
                var loc = World.LastResponse.Headers?
                    .FirstOrDefault(h => h.Name.Equals("Location", StringComparison.OrdinalIgnoreCase))?
                    .Value?.ToString();

                if (!string.IsNullOrEmpty(loc))
                {
                    var parts = loc.TrimEnd('/').Split('/');
                    World.CreatedId = parts.LastOrDefault();
                }
            }
        }
    }

    [When(@"eu fizer GET para ""(.*)""")]
    public async Task WhenGet(string path)
    {
        path = path.Replace("{idCriado}", World.CreatedId ?? "{idCriado}");
        World.LastResponse = await World.Client.Get(path);
    }

    [When(@"eu fizer DELETE para ""(.*)""")]
    public async Task WhenDelete(string path)
    {
        path = path.Replace("{idCriado}", World.CreatedId ?? "{idCriado}");
        World.LastResponse = await World.Client.Delete(path);
    }

    [Then(@"o status da resposta deve ser (.*)")]
    public void ThenStatusShouldBe(int status)
    {
        ((int)World.LastResponse!.StatusCode).Should().Be(status);
    }

    [Then(@"o corpo JSON deve obedecer ao schema ""(.*)""")]
    public void ThenBodyMatchesSchema(string schemaPath)
    {
        SchemaLoader.AssertMatches(schemaPath, World.LastResponse!.Content!);
    }

    [Then(@"o campo ""(.*)"" deve ser ""(.*)""")]
    public void ThenFieldStringEquals(string field, string expected)
    {
        using var doc = JsonDocument.Parse(World.LastResponse!.Content!);
        doc.RootElement.GetProperty(field).GetString().Should().Be(expected);
    }

    // Aceita ponto OU vírgula e faz parse com cultura invariante (fallback pt-BR)
    [Then(@"o campo ""(.*)"" deve ser ([0-9\.,\-]+)")]
    public void ThenFieldNumberEquals(string field, string expectedRaw)
    {
        // 1) Tenta Invariant (padrão JSON: ponto)
        if (!double.TryParse(expectedRaw, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var expected))
        {
            // 2) Fallback pt-BR (vírgula)
            if (!double.TryParse(expectedRaw, NumberStyles.Float | NumberStyles.AllowThousands, new CultureInfo("pt-BR"), out expected))
                throw new ArgumentException($"Valor numérico inválido para a asserção: '{expectedRaw}'.");
        }

        using var doc = JsonDocument.Parse(World.LastResponse!.Content!);
        var el = doc.RootElement.GetProperty(field);

        double actual;
        if (el.ValueKind == JsonValueKind.Number)
        {
            actual = el.GetDouble();
        }
        else if (el.ValueKind == JsonValueKind.String)
        {
            var s = el.GetString() ?? "";
            if (!double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out actual) &&
                !double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, new CultureInfo("pt-BR"), out actual))
            {
                throw new InvalidOperationException($"Campo '{field}' não é numérico (valor='{s}').");
            }
        }
        else
        {
            throw new InvalidOperationException($"Campo '{field}' não é numérico (ValueKind={el.ValueKind}).");
        }

        // Comparação robusta sem reescalar
        actual.Should().BeApproximately(expected, 1e-9, $"campo '{field}' deveria ser {expected}.");
    }

    [Given(@"que exista um descarte criado via POST em ""(.*)"" com o corpo:")]
    public async Task GivenThereIsACreatedItem(string path, string body)
    {
        await WhenPostWithBody(path, body);
        ((int)World.LastResponse!.StatusCode).Should().BeOneOf(new[] { 200, 201 });
        World.CreatedId.Should().NotBeNullOrEmpty("precisamos do ID criado para os próximos passos");
    }

    private static string? TryExtractId(JsonElement root)
    {
        if (root.ValueKind == JsonValueKind.Object)
        {
            if (root.TryGetProperty("id", out var idEl))
                return idEl.ToString();

            foreach (var p in root.EnumerateObject())
                if (p.Name.EndsWith("id", StringComparison.OrdinalIgnoreCase))
                    return p.Value.ToString();
        }
        return null;
    }
}
