using Newtonsoft.Json;

namespace ChiefPay.Models;

public sealed class RatesResponse
{
    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("data")] public List<RateData> Data { get; set; }
}

public sealed class RateData
{
    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("rate")] public decimal Rate { get; set; }
}

public sealed class CreateInvoiceRequest
{
    [JsonProperty("currency")] public required string? Currency { get; init; } = "USD";

    [JsonProperty("amount")] public required decimal? Amount { get; init; } = 0;

    [JsonProperty("description")] public string? Description { get; init; }

    [JsonProperty("orderId")] public string? OrderId { get; init; }

    [JsonProperty("accuracy")] public decimal? Accuracy { get; init; }

    [JsonProperty("feeIncluded")] public bool? FeeIncluded { get; init; }

    [JsonProperty("urlReturn")] public string? CallbackUrl { get; init; }

    [JsonProperty("urlSuccess")] public string? SuccessUrl { get; init; }
}

public sealed class CancelInvoiceRequest
{
    [JsonProperty("id")] public string? Id { get; init; }

    [JsonProperty("orderId")] public string? OrderId { get; init; }
}

public sealed class ProlongateInvoiceRequest
{
    [JsonProperty("id")] public string? Id { get; init; }

    [JsonProperty("orderId")] public string? OrderId { get; init; }
}

public sealed class CreateWalletRequest
{
    [JsonProperty("orderId")] public required string OrderId { get; init; }
}

public sealed class Invoice
{
    [JsonProperty("status")] public required string Status { get; init; }

    [JsonProperty("data")] public required InvoiceData Data { get; init; }
}

public sealed class InvoiceData
{
    [JsonProperty("id")] public required string Id { get; init; }

    [JsonProperty("orderId")] public required string OrderId { get; init; }

    [JsonProperty("description")] public string? Description { get; init; }

    [JsonProperty("amount")] public required decimal Amount { get; init; }

    [JsonProperty("payedAmount")] public decimal PaidAmount { get; init; }

    [JsonProperty("feeIncluded")] public bool FeeIncluded { get; init; }

    [JsonProperty("accuracy")] public decimal Accuracy { get; init; }

    [JsonProperty("feeRate")] public decimal FeeRate { get; init; }

    [JsonProperty("createdAt")] public DateTimeOffset CreatedAt { get; init; }

    [JsonProperty("expiredAt")] public DateTimeOffset ExpiresAt { get; init; }

    [JsonProperty("status")] public required string Status { get; init; }

    [JsonProperty("url")] public required string PaymentUrl { get; init; }

    [JsonProperty("addresses")] public List<PaymentAddress>? Addresses { get; init; }

    [JsonProperty("urlSuccess")] public string? SuccessUrl { get; init; }

    [JsonProperty("urlReturn")] public string? CallbackUrl { get; init; }

    [JsonProperty("supportLink")] public string? SupportLink { get; init; }

    [JsonProperty("merchantAmount")] public decimal MerchantAmount { get; init; }
}

public sealed class PaymentAddress
{
    [JsonProperty("chain")] public required string Chain { get; init; }

    [JsonProperty("token")] public required string Token { get; init; }

    [JsonProperty("methodName")] public required string MethodName { get; init; }

    [JsonProperty("address")] public required string Address { get; init; }

    [JsonProperty("tokenRate")] public decimal TokenRate { get; init; }
}

public sealed class LastTransaction
{
    [JsonProperty("chain")] public required string Chain { get; init; }

    [JsonProperty("txid")] public required string TransactionId { get; init; }
}

public sealed class InvoiceHistory
{
    [JsonProperty("status")] public required string Status { get; init; }

    [JsonProperty("data")] public required InvoiceHistoryData Data { get; init; }
}

public sealed class InvoiceHistoryData
{
    [JsonProperty("invoices")] public required IEnumerable<InvoiceData> Invoices { get; init; }
    
    [JsonProperty("totalCount")] public required int TotalCount { get; init; }
}

public sealed class TransactionHistory
{
    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("data")] public TransactionHistoryData Data { get; set; }
}

public sealed class TransactionHistoryData
{
    [JsonProperty("transactions")] public Transaction[] Transactions { get; set; }

    [JsonProperty("totalCount")] public int TotalCount { get; set; }
}

public sealed class Transaction
{
    [JsonProperty("txid")] public string Txid { get; set; }

    [JsonProperty("chain")] public string Chain { get; set; }

    [JsonProperty("token")] public string Token { get; set; }

    [JsonProperty("value")] public decimal Value { get; set; }

    [JsonProperty("usd")] public decimal Usd { get; set; }

    [JsonProperty("fee")] public decimal Fee { get; set; }

    [JsonProperty("wallet")] public Wallet Wallet { get; set; }

    [JsonProperty("createdAt")] public DateTime CreatedAt { get; set; }

    [JsonProperty("blockCreatedAt")] public DateTime BlockCreatedAt { get; set; }

    [JsonProperty("merchantAmount")] public decimal MerchantAmount { get; set; }
}

public class Wallet
{
    [JsonProperty("id")] public int Id { get; set; }
    
    [JsonProperty("orderId")] public string OrderId { get; set; }
}

public sealed class WalletResponse
{
    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("data")] public WalletData Data { get; set; }
}

public sealed class WalletData
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("orderId")] public string OrderId { get; set; }

    [JsonProperty("addresses")] public List<WalletAddress> Addresses { get; set; }
}

public sealed class WalletAddress
{
    [JsonProperty("chain")] public required string Chain { get; init; }

    [JsonProperty("token")] public required string Token { get; init; }

    [JsonProperty("methodName")] public required string MethodName { get; init; }

    [JsonProperty("address")] public required string Address { get; init; }
}

internal interface SocketResponseBase;

public sealed class SocketNotification : SocketResponseBase
{
    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("invoice")] public InvoiceData? Invoice { get; set; }

    [JsonProperty("transaction")] public Transaction? Transaction { get; set; }
}

[JsonConverter(typeof(SocketRatesConverter))]
public sealed class SocketRates : SocketResponseBase
{
    public IEnumerable<RateData> Rates { get; set; }
}