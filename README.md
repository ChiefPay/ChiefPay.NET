# ChiefPay .NET SDK

This is the official **.NET SDK** for integrating with the ChiefPay payment system. It provides a convenient, strongly-typed API for creating invoices, managing payments, fetching historical data, and receiving real-time notifications via **WebSocket**.

## Installation

```powershell
Install-Package ChiefPay
```

```bash
dotnet add package ChiefPay
```

## Usage

### Basic Client Setup

```csharp
using ChiefPay;
using ChiefPay.Models;

var options = new ChiefPayOptions
{
    ApiKey = "your-api-key-here",
    BaseUrl = "https://api.chiefpay.org"
};

await using var client = new ChiefPayClient(options);
```

### Create an Invoice

```csharp
var invoiceRequest = new CreateInvoiceRequest
{
    Currency    = "USD",
    Amount      = 25.50m,
    Description = "Coffee order #1001",
    OrderId     = Guid.NewGuid().ToString(),
    Accuracy    = 0.01m,
    FeeIncluded = false,
    CallbackUrl = "https://your-domain.com/webhooks/chiefpay",
    SuccessUrl  = "https://your-domain.com/thanks"
};

var createdInvoice = await client.CreateInvoiceAsync(invoiceRequest);
Console.WriteLine($"Invoice created: {createdInvoice.Id}");
```

### Invoice Management

```csharp
// Get invoice info
var invoice = await client.GetInvoiceAsync(createdInvoice.Id);

// Prolongate (extend expiration)
var prolonged = await client.ProlongateInvoiceAsync(createdInvoice.Id);

// Cancel invoice
await client.CancelInvoiceAsync(createdInvoice.Id);
```

### Historical Data & Rates

```csharp
// Invoices history
var invoicesHistory = await client.GetInvoicesHistoryAsync(new InvoicesHistoryRequest
{
    From = DateTimeOffset.UtcNow.AddDays(-7),
    To   = DateTimeOffset.UtcNow
});

// Transactions history
var txHistory = await client.GetTransactionsHistoryAsync(new TransactionsHistoryRequest
{
    From = DateTimeOffset.UtcNow.AddDays(-30),
    To   = DateTimeOffset.UtcNow
});

// Exchange rates
var rates = await client.GetRatesAsync();
Console.WriteLine($"Rates count: {rates.Count}");
```

### Wallets

```csharp
// Create wallet
var wallet = await client.CreateWalletAsync(new CreateWalletRequest
{
    Currency = "USD",
    Label    = "Primary wallet"
});

// Get wallet info
var fetchedWallet = await client.GetWalletAsync(wallet.Id);
Console.WriteLine($"Wallet balance: {fetchedWallet.Balance}");
```

### WebSocket Client (Real-Time)

```csharp
using ChiefPay;
using Newtonsoft.Json;

// Ensure the same options as your REST client
var wsOptions = new ChiefPayOptions
{
    ApiKey  = "your-api-key-here",
    BaseUrl = "https://api.chiefpay.org"
};

await using var socketClient = new ChiefPaySocketClient(wsOptions);

socketClient.OnNotification(response =>
{
    Console.WriteLine($"New payment: {JsonConvert.SerializeObject(response)}");
    return Task.CompletedTask;
});

socketClient.OnRates(response =>
{
    Console.WriteLine($"Rates updated: {JsonConvert.SerializeObject(response)}");
    return Task.CompletedTask;
});

await socketClient.ConnectAsync();
await Task.Delay(Timeout.Infinite);
```

## Requirements

- .NET 6.0 or later  
- Windows, Linux, or macOS  
- Internet access

## Support

1. API Docs: https://docs.chiefpay.org/  
2. GitHub Issues: create an issue in the package repository  
3. Technical Support: contact ChiefPay support

## License

Distributed under the **MIT** license.

## Notes

Before going to production, double-check your **webhook URL** and ensure you correctly handle **callback notifications** (signature verification, idempotency, retries, etc.).

## Examples

For comprehensive examples and advanced scenarios, check the **examples** directory (if provided by the repository).
