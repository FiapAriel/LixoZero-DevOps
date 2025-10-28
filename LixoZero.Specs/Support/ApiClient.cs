using RestSharp;

namespace LixoZero.Specs.Support
{
    public class ApiClient
    {
        private RestClient _client = null!;
        private Uri _baseUri = null!;

        public ApiClient()
        {
            var fromEnv = Environment.GetEnvironmentVariable("LIXOZERO_BASE_URL")?.Trim();
            var fallback = "http://localhost:5038";
            SetBaseUrl(string.IsNullOrWhiteSpace(fromEnv) ? fallback : fromEnv);
        }

        public ApiClient(string baseUrl)
        {
            SetBaseUrl(baseUrl);
        }

        public void SetBaseUrl(string? baseUrl)
        {
            // 1) Normaliza e filtra valores ruins/placeholder
            var candidate = (baseUrl ?? "").Trim();

            if (string.IsNullOrWhiteSpace(candidate) ||
                candidate.Contains("BASE_URL", StringComparison.OrdinalIgnoreCase) ||
                candidate.Contains("<") || candidate.Contains(">"))
            {
                candidate = Environment.GetEnvironmentVariable("LIXOZERO_BASE_URL")?.Trim();
                if (string.IsNullOrWhiteSpace(candidate))
                    candidate = "http://localhost:5038";
            }

            candidate = NormalizeBaseUrl(candidate);

            // 2) Valida como URI absoluta com esquema http/https
            if (!Uri.TryCreate(candidate, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException($"BaseUrl inválida: '{candidate}'.", nameof(baseUrl));
            }

            _baseUri = uri;

            // 3) Cria o RestClientOptions
            var options = new RestClientOptions
            {
                BaseUrl = _baseUri,
                Timeout = TimeSpan.FromSeconds(60)
            };

            _client = new RestClient(options);
            _client.AddDefaultHeader("Accept", "application/json");
        }

        // Métodos públicos 

        public async Task<RestResponse> Get(string path)
        {
            var req = new RestRequest(Resource(path), Method.Get);
            var resp = await _client.ExecuteAsync(req);
            await LogAsync(req, resp, requestBody: null);
            return resp;
        }

        public async Task<RestResponse> Delete(string path)
        {
            var req = new RestRequest(Resource(path), Method.Delete);
            var resp = await _client.ExecuteAsync(req);
            await LogAsync(req, resp, requestBody: null);
            return resp;
        }

        public async Task<RestResponse> PostRaw(string path, string jsonBody)
        {
            var req = new RestRequest(Resource(path), Method.Post);
            // Compatível com versões recentes do RestSharp
            req.AddStringBody(jsonBody, "application/json");

            var resp = await _client.ExecuteAsync(req);
            await LogAsync(req, resp, requestBody: jsonBody);
            return resp;
        }

        // helpers 

        private string Resource(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return "";
            var p = path.Trim();

            if (Uri.TryCreate(p, UriKind.Absolute, out _)) return p;

            return p.TrimStart('/');
        }

        private async Task LogAsync(RestRequest req, RestResponse resp, string? requestBody)
        {
            // Uri final (se o RestSharp não devolver, monta a partir da base)
            var final = resp.ResponseUri ?? new Uri(_baseUri, req.Resource ?? "");

            // Console curto (pra ver no output do runner)
            Console.WriteLine($"[HTTP] {req.Method} {final} -> {(int)resp.StatusCode} {resp.StatusCode}");
            if (!resp.IsSuccessful)
            {
                Console.WriteLine($"[HTTP] Content: {resp.Content}");
                if (!string.IsNullOrWhiteSpace(resp.ErrorMessage))
                    Console.WriteLine($"[HTTP] Error: {resp.ErrorMessage}");
            }

            // Log completo (sem cortes) no arquivo via TestLog
            // Obs.: resp.Content pode ser null em 204/HEAD; tratar como "<null>"
            var content = resp.Content ?? "<null>";
            var reqBodyForLog = string.IsNullOrWhiteSpace(requestBody) ? "<null>" : requestBody;

            await TestLog.WriteAsync($@"[HTTP] {req.Method} {final}
Status: {(int)resp.StatusCode} {resp.StatusCode}
RequestBody: {reqBodyForLog}
ResponseBody: {content}");
        }

        private static string NormalizeBaseUrl(string baseUrl)
        {
            var b = (baseUrl ?? string.Empty).Trim();
            if (!b.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !b.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                b = "http://" + b.TrimStart('/');

            return b.TrimEnd('/');
        }
    }
}
