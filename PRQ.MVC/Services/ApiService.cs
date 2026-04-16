using System.Net.Http.Json;
using System.Text.Json;

namespace PRQ.MVC.Services;

public class ApiService(HttpClient httpClient, IApiRequestSourceResolver requestSourceResolver) : IApiService
{
    public async Task<T?> GetAsync<T>(string url)
    {
        using var request = CreateRequest(HttpMethod.Get, url);
        using var response = await httpClient.SendAsync(request);
        await EnsureSuccessStatusCodeAsync(response);

        return await DeserializeResponseAsync<T>(response);
    }

    public async Task<T?> PostAsync<T>(string url, object data)
    {
        using var request = CreateRequest(HttpMethod.Post, url, data);
        using var response = await httpClient.SendAsync(request);
        await EnsureSuccessStatusCodeAsync(response);

        return await DeserializeResponseAsync<T>(response);
    }

    public async Task<T?> PutAsync<T>(string url, object data)
    {
        using var request = CreateRequest(HttpMethod.Put, url, data);
        using var response = await httpClient.SendAsync(request);
        await EnsureSuccessStatusCodeAsync(response);

        return await DeserializeResponseAsync<T>(response);
    }

    public async Task DeleteAsync(string url)
    {
        using var request = CreateRequest(HttpMethod.Delete, url);
        using var response = await httpClient.SendAsync(request);
        await EnsureSuccessStatusCodeAsync(response);
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string url, object? data = null)
    {
        var request = new HttpRequestMessage(method, url);
        var sourceSelection = requestSourceResolver.ResolveCurrent();

        request.Headers.TryAddWithoutValidation(sourceSelection.HeaderName, sourceSelection.Source);

        if (data is not null)
        {
            request.Content = JsonContent.Create(data);
        }

        return request;
    }

    private static async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"API request failed with status code {(int)response.StatusCode}: {content}",
            null,
            response.StatusCode);
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