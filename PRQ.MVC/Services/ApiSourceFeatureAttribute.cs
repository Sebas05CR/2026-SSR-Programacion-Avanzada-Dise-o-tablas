namespace PRQ.MVC.Services;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ApiSourceFeatureAttribute(string featureName) : Attribute
{
    public string FeatureName { get; } = featureName;
}