using Json.Schema;
using System.Text.Json;
using FluentAssertions;

namespace LixoZero.Specs.Support;

public static class SchemaLoader
{
    public static void AssertMatches(string schemaPath, string jsonContent)
    {
        if (string.IsNullOrWhiteSpace(jsonContent))
            throw new Exception("Resposta vazia; não é possível validar o contrato.");

        var fullPath = Resolve(schemaPath);
        var schemaText = File.ReadAllText(fullPath);

        var schema = JsonSchema.FromText(schemaText);
        using var doc = JsonDocument.Parse(jsonContent);
        var result = schema.Evaluate(doc.RootElement, new EvaluationOptions { OutputFormat = OutputFormat.List });

        result.IsValid.Should().BeTrue($"Violação de contrato em {schemaPath}");
    }

    private static string Resolve(string relative)
    {
        var try1 = Path.Combine(AppContext.BaseDirectory, relative);
        if (File.Exists(try1)) return try1;
        var try2 = Path.Combine(Directory.GetCurrentDirectory(), relative);
        if (File.Exists(try2)) return try2;
        var try3 = Path.Combine(Directory.GetCurrentDirectory(), "LixoZero.Specs", relative);
        if (File.Exists(try3)) return try3;
        throw new FileNotFoundException($"Schema não encontrado: {relative}");
    }
}
