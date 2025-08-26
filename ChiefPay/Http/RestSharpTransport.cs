using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace ChiefPay.Http;

internal sealed class RestSharpTransport : IDisposable
{
    private readonly ChiefPayOptions _options;
    private readonly RestClient _client;

    public RestSharpTransport(ChiefPayOptions options)
    {
        _options = options;
        var baseUrl = options.BaseUrl.TrimEnd('/') + "/";

        var opts = new RestClientOptions(baseUrl)
        {
            ThrowOnAnyError = false,
            FollowRedirects = true
        };
        _client = new RestClient(opts);

        _client.AddDefaultHeader("Accept", "application/json");
        var ua = "ChiefPay-CSharp/1.0 (+https://chiefpay.org)" + (string.IsNullOrWhiteSpace(options.UserAgentSuffix)
            ? string.Empty
            : $" {options.UserAgentSuffix}");
        _client.AddDefaultHeader("User-Agent", ua);
    }

    public async Task<T> SendAsync<T>(Method method, string path, object? body = null,
        IDictionary<string, string?>? query = null, CancellationToken ct = default)
    {
        var request = new RestRequest(path, method);
        request.AddHeader("x-api-key", _options.ApiKey);

        if (query is { Count: > 0 })
        {
            foreach (var kv in query)
            {
                request.AddQueryParameter(kv.Key, kv.Value ?? string.Empty);
            }
        }

        if (body is not null)
        {
            var jsonBody = JsonConvert.SerializeObject(body);

            request.AddStringBody(jsonBody, DataFormat.Json);
        }

        var attempt = 0;
        while (true)
        {
            attempt++;
            try
            {
                var resp = await _client.ExecuteAsync(request, ct);
                if (resp is { IsSuccessful: true, Content: not null })
                {
                    if (typeof(T) == typeof(string))
                    {
                        return (T)(object)resp.Content;
                    }

                    return JsonConvert.DeserializeObject<T>(resp.Content)!;
                }

                var status = resp.StatusCode;
                if (ShouldRetry(status) && attempt <= _options.MaxRetries)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3 * attempt), ct);
                    continue;
                }

                ThrowIfConfigured(resp);
                return default!;
            }
            catch (TaskCanceledException) when (!ct.IsCancellationRequested && attempt <= _options.MaxRetries)
            {
                await Task.Delay(TimeSpan.FromSeconds(3 * attempt), ct).ConfigureAwait(false);
            }
        }
    }

    private static bool ShouldRetry(HttpStatusCode status) =>
        status == HttpStatusCode.RequestTimeout ||
        (int)status == 429 ||
        ((int)status >= 500 && (int)status <= 599);

    private static void ThrowIfConfigured(RestResponse resp)
    {
        var status = (int)resp.StatusCode;
        var message = $"HTTP {status} {resp.StatusDescription}. Body: {resp.Content}";
        throw new Exceptions.APIError(message, status,
            Exceptions.ChiefPayErrorCodeExtensions.ChiefPayErrorCodeFromStatus(resp.StatusCode), resp.Content);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}