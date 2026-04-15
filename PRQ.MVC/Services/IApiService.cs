namespace PRQ.MVC.Services;

public interface IApiService
{
    Task<T?> GetAsync<T>(string url);
    Task<T?> PostAsync<T>(string url, object data);
    Task<T?> PutAsync<T>(string url, object data);
    Task DeleteAsync(string url);
}