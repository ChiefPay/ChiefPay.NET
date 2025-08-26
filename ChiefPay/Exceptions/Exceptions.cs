using System.Net;

namespace ChiefPay.Exceptions;

public enum ChiefPayErrorCode
{
    Unknown = 0,
    InvalidRequest = 1,
    Unauthorized = 2,
    PermissionDenied = 3,
    NotFound = 4,
    Conflict = 5,
    TooManyRequests = 6,
    InternalError = 7,
    BadGateway = 8,
    ServiceUnavailable = 9,
}

public static class ChiefPayErrorCodeExtensions
{
    public static ChiefPayErrorCode ChiefPayErrorCodeFromStatus(HttpStatusCode status)
    {
        return status switch
        {
            HttpStatusCode.BadRequest => ChiefPayErrorCode.InvalidRequest,
            HttpStatusCode.Unauthorized => ChiefPayErrorCode.Unauthorized,
            HttpStatusCode.Forbidden => ChiefPayErrorCode.PermissionDenied,
            HttpStatusCode.NotFound => ChiefPayErrorCode.NotFound,
            HttpStatusCode.Conflict => ChiefPayErrorCode.Conflict,
            HttpStatusCode.TooManyRequests => ChiefPayErrorCode.TooManyRequests,
            HttpStatusCode.InternalServerError => ChiefPayErrorCode.InternalError,
            HttpStatusCode.BadGateway => ChiefPayErrorCode.BadGateway,
            HttpStatusCode.ServiceUnavailable => ChiefPayErrorCode.ServiceUnavailable,
            _ => ChiefPayErrorCode.Unknown
        };
    }
}

public class ChiefPayException : Exception
{
    public ChiefPayException(string message) : base(message)
    {
    }
}

public sealed class APIError : ChiefPayException
{
    public int StatusCode { get; }
    public ChiefPayErrorCode Code { get; }
    public string? Body { get; }

    public APIError(string message, int statusCode, ChiefPayErrorCode code, string? body)
        : base(message)
    {
        StatusCode = statusCode;
        Code = code;
        Body = body;
    }
}

public sealed class SocketException : Exception
{
    public SocketException(string message) : base(message)
    {
    }

    public SocketException(string message, Exception innerException) : base("", innerException)
    {
    }
}