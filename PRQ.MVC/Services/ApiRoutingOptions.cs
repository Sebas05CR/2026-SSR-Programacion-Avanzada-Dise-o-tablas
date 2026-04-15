namespace PRQ.MVC.Services;

public class ApiRoutingOptions
{
    public const string SectionName = "ApiRouting";

    public string HeaderName { get; set; } = "X-PRQ-Source";
    public string DefaultSource { get; set; } = "DB";
    public Dictionary<string, string> Features { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}