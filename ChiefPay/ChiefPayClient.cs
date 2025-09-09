using ChiefPay.Http;
using ChiefPay.Models;
using RestSharp;

namespace ChiefPay;

public sealed class ChiefPayClient : IAsyncDisposable
{
    private readonly RestSharpTransport _http;

    public ChiefPayClient(ChiefPayOptions options)
    {
        _http = new RestSharpTransport(options);
    }

    public Task<RatesResponse> GetRatesAsync(CancellationToken ct = default) =>
        _http.SendAsync<RatesResponse>(Method.Get, "v1/rates", null, null, ct);

    public Task<Invoice> CreateInvoiceAsync(CreateInvoiceRequest req, string? idempotencyKey = null,
        CancellationToken ct = default) =>
        _http.SendAsync<Invoice>(Method.Post, "v1/invoice", req, null, ct);

    public Task<Invoice> CancelInvoiceAsync(CancelInvoiceRequest req, string? idempotencyKey = null,
        CancellationToken ct = default)
    {
        if (req.Id == null && req.OrderId == null)
            throw new ArgumentException("Either 'Id' or 'OrderId' must be provided.");

        return _http.SendAsync<Invoice>(Method.Delete, "v1/invoice", req, null, ct);
    }

    public Task<Invoice> ProlongateInvoiceAsync(ProlongateInvoiceRequest req,
        string? idempotencyKey = null,
        CancellationToken ct = default)
    {
        if (req.Id == null && req.OrderId == null)
            throw new ArgumentException("Either 'Id' or 'OrderId' must be provided.");


        return _http.SendAsync<Invoice>(Method.Patch, "v1/invoice", req, null, ct);
    }

    public Task<Invoice> GetInvoiceAsync(string invoiceId, CancellationToken ct = default) =>
        _http.SendAsync<Invoice>(Method.Get, $"v1/invoice?id={invoiceId}", null, null, ct);

    public Task<InvoiceHistory> GetInvoicesHistoryAsync(DateTime fromDate, DateTime toDate, int limit,
        CancellationToken ct = default)
    {
        var fromDateFormatted = fromDate.ToString("o", System.Globalization.CultureInfo.InvariantCulture);
        var toDateFormatted = toDate.ToString("o", System.Globalization.CultureInfo.InvariantCulture);

        var url =
            $"/v1/history/invoices?fromDate={Uri.EscapeDataString(fromDateFormatted)}&toDate={Uri.EscapeDataString(toDateFormatted)}&limit={limit}";

        return _http.SendAsync<InvoiceHistory>(Method.Get, url, null, null, ct);
    }

    public Task<TransactionHistory> GetTransactionsHistoryAsync(DateTime fromDate, DateTime toDate, int limit,
        CancellationToken ct = default)
    {
        var fromDateFormatted = fromDate.ToString("o", System.Globalization.CultureInfo.InvariantCulture);
        var toDateFormatted = toDate.ToString("o", System.Globalization.CultureInfo.InvariantCulture);

        var url =
            $"/v1/history/transactions?fromDate={Uri.EscapeDataString(fromDateFormatted)}&toDate={Uri.EscapeDataString(toDateFormatted)}&limit={limit}";

        return _http.SendAsync<TransactionHistory>(Method.Get, url, null, null, ct);
    }

    public Task<WalletResponse> GetWalletAsync(string? id = null, string? orderId = null,
        CancellationToken ct = default)
    {
        if (id == null && orderId == null)
            throw new ArgumentException("Either 'id' or 'orderId' must be provided.");

        var query = id != null
            ? $"id={Uri.EscapeDataString(id)}"
            : $"orderId={Uri.EscapeDataString(orderId!)}";

        return _http.SendAsync<WalletResponse>(
            Method.Get,
            $"/v1/wallet?{query}",
            null,
            null,
            ct
        );
    }


    public Task<WalletResponse> CreateWalletAsync(CreateWalletRequest req, CancellationToken ct = default) =>
        _http.SendAsync<WalletResponse>(Method.Post, "/v1/wallet", req, null, ct);

    public ValueTask DisposeAsync()
    {
        _http.Dispose();
        return ValueTask.CompletedTask;
    }
}