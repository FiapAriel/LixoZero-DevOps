using RestSharp;

namespace LixoZero.Specs.Support;

public static class World
{
    public static string BaseUrl = Environment.GetEnvironmentVariable("LIXOZERO_BASE_URL") ?? "http://localhost:5038";
    public static ApiClient Client = new ApiClient(BaseUrl);

    public static RestResponse? LastResponse;
    public static string? CreatedId;
}
