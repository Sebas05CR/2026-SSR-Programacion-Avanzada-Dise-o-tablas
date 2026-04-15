using System.Net.Http.Json;
using System.Text.Json;

namespace PRQ.MVC.Services;

public class ApiService(HttpClient httpClient) : IApiService
{
    public async Task<T?> GetAsync<T>(string url)
    {
        using var response = await httpClient.GetAsync(url);
        await EnsureSuccessStatusCodeAsync(response);

        return await DeserializeResponseAsync<T>(response);
    }

    public async Task<T?> PostAsync<T>(string url, object data)
    {
        using var response = await httpClient.PostAsJsonAsync(url, data);
        await EnsureSuccessStatusCodeAsync(response);

        return await DeserializeResponseAsync<T>(response);
    }

    public async Task<T?> PutAsync<T>(string url, object data)
    {
        using var response = await httpClient.PutAsJsonAsync(url, data);
        await EnsureSuccessStatusCodeAsync(response);

        return await DeserializeResponseAsync<T>(response);
    }

    public async Task DeleteAsync(string url)
    {
        using var response = await httpClient.DeleteAsync(url);
        await EnsureSuccessStatusCodeAsync(response);
    }

    private static async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"API request failed with status code {(int)response.StatusCode}: {content}");
    }

    private static async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response)
    {
        if (response.Content.Headers.ContentLength is 0)
        {
            return default;
        }

        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(content))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }
}