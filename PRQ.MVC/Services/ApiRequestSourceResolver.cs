using Microsoft.Extensions.Options;

namespace PRQ.MVC.Services;

public interface IApiRequestSourceResolver
{
    ApiRequestSourceSelection ResolveCurrent();
}

public sealed record ApiRequestSourceSelection(string HeaderName, string Source);

public class ApiRequestSourceResolver(
    IHttpContextAccessor httpContextAccessor,
    IOptions<ApiRoutingOptions> options) : IApiRequestSourceResolver
{
    public ApiRequestSourceSelection ResolveCurrent()
    {
        var routingOptions = options.Value;
        var endpoint = httpContextAccessor.HttpContext?.GetEndpoint();
        var featureAttribute = endpoint?.Metadata.GetMetadata<ApiSourceFeatureAttribute>();

        var source = routingOptions.DefaultSource;
        if (featureAttribute is not null &&
            routingOptions.Features.TryGetValue(featureAttribute.FeatureName, out var configuredSource) &&
            !string.IsNullOrWhiteSpace(configuredSource))
        {
            source = configuredSource;
        }

        return new ApiRequestSourceSelection(routingOptions.HeaderName, source);
    }
}