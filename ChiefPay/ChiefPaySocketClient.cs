using ChiefPay.Exceptions;
using ChiefPay.Models;
using Newtonsoft.Json;
using SocketIOClient;
using SocketIOClient.Transport;

namespace ChiefPay;

public class ChiefPaySocketClient : IAsyncDisposable
{
    private readonly SocketIOClient.SocketIO? _client;

    public ChiefPaySocketClient(ChiefPayOptions options, EventHandler? onConnected = null,
        EventHandler<string>? onDisconnected = null,
        EventHandler<string>? onError = null, TimeSpan? connectionTimeout = null, int reconnectionAttempts = 3,
        double reconnectionDelay = 5000)
    {
        var socketOptions = new SocketIOOptions
        {
            Path = "/socket.io",
            ConnectionTimeout = connectionTimeout ?? _defaultConnectionTimeout,
            ReconnectionAttempts = reconnectionAttempts,
            ReconnectionDelay = reconnectionDelay,
            Transport = TransportProtocol.WebSocket,
            ExtraHeaders = new Dictionary<string, string>
            {
                { "x-api-key", options.ApiKey },
            },
        };

        _client = new SocketIOClient.SocketIO(options.BaseUrl, socketOptions);

        _client.OnConnected += onConnected ?? DefaultOnConnected;
        _client.OnDisconnected += onDisconnected ?? DefaultOnDisconnected;
        _client.OnError += onError ?? DefaultOnError;
    }

    private EventHandler DefaultOnConnected => (_, _) => Console.WriteLine("Connected!!!");

    private EventHandler<string> DefaultOnDisconnected => (_, e) => Console.WriteLine($"Disconnected!!! Error: {e}");

    private EventHandler<string> DefaultOnError => (_, e) => Console.WriteLine($"Error: {e}");

    private readonly TimeSpan _defaultConnectionTimeout = TimeSpan.FromSeconds(30);

    public async Task ConnectAsync(CancellationToken ct = default)
    {
        if (_client == null || _client.Connected) return;

        await _client.ConnectAsync(ct);
    }

    public void OnNotification(Func<SocketNotification, Task> action)
    {
        OnEvent("notification", action, true);
    }

    private void OnEvent<T>(string eventName, Func<T, Task> action, bool needCallback) where T : SocketResponseBase
    {
        if (_client != null)
            _client.On(eventName, async response =>
            {
                try
                {
                    var rawJson = response.ToString();

                    var deserialized = JsonConvert.DeserializeObject<IEnumerable<T>>(rawJson);

                    if (deserialized == null)
                        throw new SocketException("Response has bad format");

                    await action(deserialized.First());

                    if (needCallback)
                    {
                        await response.CallbackAsync(new { status = "success" });
                    }
                }
                catch (Exception e)
                {
                    if (needCallback)
                        await response.CallbackAsync(new { status = "error" });
                    throw new SocketException("Cannot get socket response.", e);
                }
            });
    }

    public void OnRates(Func<SocketRates, Task> action)
    {
        OnEvent("rates", action, false);
    }

    public async ValueTask DisposeAsync()
    {
        if (_client != null)
        {
            if (_client.Connected)
                await _client.DisconnectAsync();

            _client.Dispose();
        }
    }
}