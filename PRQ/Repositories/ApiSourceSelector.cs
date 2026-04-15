namespace PRQ.Repositories;

public class ApiSourceSelectorOptions
{
    public const string DefaultHeaderName = "X-PRQ-Source";

    public string HeaderName { get; set; } = DefaultHeaderName;
    public string DefaultSource { get; set; } = "DB";
}

public interface IApiSourceSelector
{
    bool UseJsonSource();
}

public class ApiSourceSelector(
    IHttpContextAccessor httpContextAccessor,
    ApiSourceSelectorOptions options) : IApiSourceSelector
{
    public bool UseJsonSource()
    {
        var request = httpContextAccessor.HttpContext?.Request;
        if (request is not null &&
            request.Headers.TryGetValue(options.HeaderName, out var values) &&
            !string.IsNullOrWhiteSpace(values.ToString()))
        {
            return values.ToString().Equals("JSON", StringComparison.OrdinalIgnoreCase);
        }

        return options.DefaultSource.Equals("JSON", StringComparison.OrdinalIgnoreCase);
    }
}