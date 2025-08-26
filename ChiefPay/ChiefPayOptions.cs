namespace ChiefPay;

public sealed class ChiefPayOptions
{
    public required string ApiKey { get; init; }
    public string BaseUrl { get; init; } = "https://api.chiefpay.org";
    public string? UserAgentSuffix { get; init; }

    public int? MaxRetries { get; init; } = 5;
}