using System;

namespace api.Errors;

public class ApiException(int statusCode, string msg, string? details)
{
    public int StatusCode { get; set; } = statusCode;
    public string Message { get; set; } = msg;
    public string? Details { get; set; } = details;
}
